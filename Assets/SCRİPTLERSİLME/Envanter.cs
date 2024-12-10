using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EnvanterYonetimi : MonoBehaviour
{
    public enum ItemTuru
    {
        Yenebilir,
        İçilebilir,
        İyileştirilebilir,
        Silah,
        Kask,
        Zırh,
        Ayakkabı,
        Diğer
    }

   [System.Serializable]
public class Item
{
    public string itemAdi;
    public ItemTuru itemTuru;
    public int itemMiktari;
    public Button itemButon;
    public GameObject itemOyunObjesi;
    public TextMeshProUGUI itemMiktarText;

    // Karakter durumu üzerindeki etkiler
    public float aciklikEtkisi = 0f;   // Açlık etkisi
    public float susuzlukEtkisi = 0f;  // Susuzluk etkisi
    public float mutlulukEtkisi = 0f;  // Mutluluk etkisi
    public float canEtkisi = 0f;       // Can etkisi
    public float savunmaEtkisi = 0f;   // Savunma etkisi
    public float hizEtkisi = 0f;       // Hız etkisi
}
    public List<Item> envanterItemleri;
    public RectTransform panelRect;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public GridLayoutGroup gridLayout;
    public GameObject miktarPaneli;
    public Slider miktarSlider;
    public TextMeshProUGUI miktarText;
    public Button onaylaButonu;
    public float enYakinMesafeEşiği = 200f;

    public int maksimumItemMiktari = 100;
    public TextMeshProUGUI toplamItemText;
    private int toplamItemMiktari;

    private Dictionary<Button, Item> butonItemMap = new Dictionary<Button, Item>();
    private Button seciliButon;
    private Vector2 ilkPozisyon;
    private int orijinalIndex;
    private Item azaltilacakItem;
    private float sonTiklamaZamani;
    private Item sonTiklananItem;
    public float ciftTiklamaSuresi = 0.3f;
    public KarakterDurum karakterDurum;

    private void Start()
    {
        miktarPaneli.SetActive(false);
        ToplamItemMiktariniGuncelle();

        foreach (Item item in envanterItemleri)
        {
            if (item.itemButon == null || item.itemMiktarText == null)
            {
                Debug.LogError($"Item hatalı: {item.itemAdi}. itemButon veya itemMiktarText eksik!");
                continue;
            }

            butonItemMap[item.itemButon] = item;
            item.itemButon.onClick.AddListener(() => ItemButonTiklama(item));

            if (item.itemMiktarText != null)
            {
                item.itemMiktarText.text = $"{item.itemAdi} ({item.itemMiktari})";
            }
        }

        onaylaButonu.onClick.AddListener(OnaylaMiktarAzaltma);
    }

    private void AzaltItemMiktari(Item item)
    {
        item.itemMiktari--;
        if (item.itemMiktarText != null)
        {
            if (item.itemMiktari <= 0)
            {
                item.itemButon.gameObject.SetActive(false);
            }
            else
            {
                item.itemMiktarText.text = $"{item.itemAdi} ({item.itemMiktari})";
            }
        }
    }

    private void ItemButonTiklama(Item item)
    {
        float gecenSure = Time.time - sonTiklamaZamani;

        if (item == sonTiklananItem && gecenSure < ciftTiklamaSuresi)
        {
            // Çift tıklama algılandı
            ItemiKullan(item);
            sonTiklamaZamani = 0f;
            sonTiklananItem = null;
        }
        else
        {
            // Tek tıklama
            sonTiklamaZamani = Time.time;
            sonTiklananItem = item;
        }
    }

    private void ItemiKullan(Item item)
    {
        // Eğer item türü "Diğer" ise, itemin miktarını azaltma
        if (item.itemTuru == ItemTuru.Diğer)
        {
            return;
        }

        switch (item.itemTuru)
        {
            case ItemTuru.Yenebilir:
                karakterDurum.DegistirAclik(item.aciklikEtkisi);
                karakterDurum.DegistirMutluluk(item.mutlulukEtkisi);
                AzaltItemMiktari(item);
                break;

            case ItemTuru.İçilebilir:
                karakterDurum.DegistirSusuzluk(item.susuzlukEtkisi);
                karakterDurum.DegistirMutluluk(item.mutlulukEtkisi);
                AzaltItemMiktari(item);
                break;

            case ItemTuru.İyileştirilebilir:
                karakterDurum.DegistirCan(item.canEtkisi);
                AzaltItemMiktari(item);
                break;

            case ItemTuru.Silah:
            case ItemTuru.Kask:
            case ItemTuru.Zırh:
            case ItemTuru.Ayakkabı:
                EkipmanKullan(item);
                // Ekipman türündeki itemler için AzaltItemMiktari çağrılmıyor
                break;
        }

        ToplamItemMiktariniGuncelle();
    }


   private void EkipmanKullan(Item item)
{
    // Diğer aynı türdeki ekipmanları deaktif et
    foreach (Item digerItem in envanterItemleri)
    {
        if (digerItem.itemTuru == item.itemTuru && digerItem != item)
        {
            if (digerItem.itemOyunObjesi != null)
            {
                digerItem.itemOyunObjesi.SetActive(false);
            }
        }
    }

    // Seçilen ekipmanı aktifleştir ve etkilerini uygula
    if (item.itemOyunObjesi != null)
    {
        bool yeniDurum = !item.itemOyunObjesi.activeSelf;
        item.itemOyunObjesi.SetActive(yeniDurum);

        // Ekipman aktifleştirildiğinde etkileri uygula
        if (yeniDurum)
        {
            switch (item.itemTuru)
            {
                case ItemTuru.Silah:
                    // Silah etkisi eklenebilir
                    break;
                case ItemTuru.Kask:
                case ItemTuru.Zırh:
                    karakterDurum.DegistirSavunma(item.savunmaEtkisi);
                    break;
                case ItemTuru.Ayakkabı:
                    karakterDurum.DegistirHiz(item.hizEtkisi);
                    break;
            }
        }
        // Ekipman deaktif edildiğinde etkileri kaldır
        else
        {
            switch (item.itemTuru)
            {
                case ItemTuru.Silah:
                    // Silah etkisini kaldır
                    break;
                case ItemTuru.Kask:
                case ItemTuru.Zırh:
                    karakterDurum.DegistirSavunma(-item.savunmaEtkisi);
                    break;
                case ItemTuru.Ayakkabı:
                    karakterDurum.DegistirHiz(-item.hizEtkisi);
                    break;
            }
        }
    }
}


    private void ToplamItemMiktariniGuncelle()
    {
        toplamItemMiktari = 0;
        foreach (Item item in envanterItemleri)
        {
            toplamItemMiktari += item.itemMiktari;
        }

        if (toplamItemMiktari >= maksimumItemMiktari)
        {
            toplamItemMiktari = maksimumItemMiktari;
        }

        toplamItemText.text = $" {toplamItemMiktari} / {maksimumItemMiktari}";
    }
private void ButonTasima(Button buton, Vector2 hareket)
{
    // OnaylaButon'un taşınmasını engelle
    if (buton == onaylaButonu) return;

    RectTransform butonRect = buton.GetComponent<RectTransform>();
    butonRect.anchoredPosition += hareket;
}



    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch dokunma = Input.GetTouch(0);

            switch (dokunma.phase)
            {
                case TouchPhase.Began:
                    DokunulanButonuTespitEt(dokunma.position);
                    gridLayout.enabled = false;
                    break;

                case TouchPhase.Moved:
                    if (seciliButon != null)
                    {
                        Vector2 hareket = dokunma.position - ilkPozisyon;
                        ButonTasima(seciliButon, hareket);

                        // Diğer butonlarla yer değiştirme kontrolü
                        YerDegistirmeKontrol(seciliButon);
                    }
                    break;

                case TouchPhase.Ended:
                    if (seciliButon != null)
                    {
                        PanelDisiKontrolEt(seciliButon, dokunma.position);
                        seciliButon = null;
                        gridLayout.enabled = true;
                    }
                    break;
            }

            ilkPozisyon = dokunma.position;
        }
    }

    private void YerDegistirmeKontrol(Button hareketEdenButon)
    {
        RectTransform hareketRect = hareketEdenButon.GetComponent<RectTransform>();

        foreach (Transform cocuk in gridLayout.transform)
        {
            if (cocuk.gameObject == hareketEdenButon.gameObject) continue;

            RectTransform cocukRect = cocuk.GetComponent<RectTransform>();

            // Butonların mesafelerini kontrol et
            float mesafe = Vector2.Distance(hareketRect.anchoredPosition, cocukRect.anchoredPosition);

            if (mesafe < enYakinMesafeEşiği)
            {
                // Yer değiştirme
                int hareketIndex = hareketEdenButon.transform.GetSiblingIndex();
                int hedefIndex = cocuk.transform.GetSiblingIndex();

                hareketEdenButon.transform.SetSiblingIndex(hedefIndex);
                cocuk.transform.SetSiblingIndex(hareketIndex);

                break;
            }
        }
    }

   private void DokunulanButonuTespitEt(Vector2 dokunmaPozisyonu)
{
    PointerEventData pointerData = new PointerEventData(eventSystem)
    {
        position = dokunmaPozisyonu
    };

    var sonuclar = new List<RaycastResult>();
    raycaster.Raycast(pointerData, sonuclar);

    foreach (var sonuc in sonuclar)
    {
        Button buton = sonuc.gameObject.GetComponent<Button>();

        // Eğer buton OnaylaButon değilse, taşınabilir
        if (buton != null && buton != onaylaButonu && buton.transform.parent == gridLayout.transform)
        {
            seciliButon = buton;

            // İlk pozisyonu kesin şekilde kaydet
            RectTransform butonRect = buton.GetComponent<RectTransform>();
            ilkPozisyon = butonRect.anchoredPosition;

            // Sıralama indeksini kaydet
            orijinalIndex = buton.transform.GetSiblingIndex();

            break;
        }
    }
}


   private void PanelDisiKontrolEt(Button buton, Vector2 dokunmaPozisyonu)
{
    RectTransform butonRect = buton.GetComponent<RectTransform>();

    // Eğer buton panelin dışına çıktıysa, işlemi kontrol et
    if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, butonRect.position))
    {
        // Paneli aç ve item miktarı azaltma işlemini yapma
        if (butonItemMap.TryGetValue(buton, out Item item))
        {
            azaltilacakItem = item;

            // Miktar azaltma panelini aç
            miktarSlider.maxValue = item.itemMiktari;
            miktarSlider.value = 1;
            miktarText.text = "Azaltılacak Miktar: 1";
            miktarPaneli.SetActive(true);
            gridLayout.enabled = false;

            miktarSlider.onValueChanged.RemoveAllListeners();
            miktarSlider.onValueChanged.AddListener(deger =>
            {
                miktarText.text = $"Azaltılacak Miktar: {(int)deger}";
            });
        }

        // Butonu eski pozisyonuna geri getirme
        buton.GetComponent<RectTransform>().anchoredPosition = ilkPozisyon;

        // Orijinal indeks sırasını geri yükle
        buton.transform.SetSiblingIndex(orijinalIndex);

        // GridLayoutGroup'u yeniden etkinleştir
        gridLayout.enabled = true;
    }
    else
    {
        // Eğer panel içindeyse ve itemi azalttıysak, miktar azaltma işlemini yap
        if (azaltilacakItem != null)
        {
            int azaltmaMiktari = (int)miktarSlider.value;
            azaltilacakItem.itemMiktari -= azaltmaMiktari;

            // Eğer itemin miktarı sıfırsa, buton gizlenir
            if (azaltilacakItem.itemMiktari <= 0)
            {
                azaltilacakItem.itemMiktari = 0;
                azaltilacakItem.itemButon.gameObject.SetActive(false);
            }
            else
            {
                if (azaltilacakItem.itemMiktarText != null)
                {
                    azaltilacakItem.itemMiktarText.text = $"{azaltilacakItem.itemAdi} ({azaltilacakItem.itemMiktari})";
                }
            }

            // Toplam item miktarını güncelle
            ToplamItemMiktariniGuncelle();
            miktarPaneli.SetActive(false);
            gridLayout.enabled = true;
            azaltilacakItem = null;
        }
    }
}


  private void OnaylaMiktarAzaltma()
{
    if (azaltilacakItem != null)
    {
        // Slider'dan seçilen miktarı al
        int azaltmaMiktari = (int)miktarSlider.value;

        // Itemin miktarını azalt
        azaltilacakItem.itemMiktari -= azaltmaMiktari;

        // Eğer item miktarı sıfır veya daha az olduysa, butonu gizle
        if (azaltilacakItem.itemMiktari <= 0)
        {
            azaltilacakItem.itemMiktari = 0;
            azaltilacakItem.itemButon.gameObject.SetActive(false);  // Buton gizlenir
        }
        else
        {
            // Miktar sıfırdan büyükse, miktar textini güncelle
            if (azaltilacakItem.itemMiktarText != null)
            {
                azaltilacakItem.itemMiktarText.text = $"{azaltilacakItem.itemAdi} ({azaltilacakItem.itemMiktari})";
            }
        }

        // Toplam item miktarını güncelle
        ToplamItemMiktariniGuncelle();

        // Paneli kapat ve grid layout'u yeniden aktif hale getir
        miktarPaneli.SetActive(false);
        gridLayout.enabled = true;

        // Azaltılacak item'i null yaparak işlemi bitir
        azaltilacakItem = null;
    }
}

}
