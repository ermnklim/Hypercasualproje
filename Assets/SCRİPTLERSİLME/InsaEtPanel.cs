using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ButonObjeCifti
{
    public Button uretbuton, onizlemebuton, sagadondurbuton, soladondurbuton;
    public GameObject onizlemeObje;
    public GameObject onizlemePanel; // Yeni eklenen panel
    public List<GameObject> UretilecekObjeler;
    public int objemiktar, maxobjemiktar;
    public string[] ihtiyacobjeisimleri;
    public int[] ihtiyacobjemiktarlari;
    public TextMeshProUGUI miktarText;
    public List<TextMeshProUGUI> ihtiyacTextListesi;
    public TriggerKontrol triggerKontrol;
}


public class InsaEtPanel : MonoBehaviour
{
    public List<ButonObjeCifti> butonObjeCiftleri;
    private EnvanterYonetimi envanterYonetimi;

    private void Start()
    {
        envanterYonetimi = FindObjectOfType<EnvanterYonetimi>();

        foreach (var butonObjeCifti in butonObjeCiftleri)
        {
            butonObjeCifti.maxobjemiktar = butonObjeCifti.UretilecekObjeler.Count;

            butonObjeCifti.onizlemebuton.onClick.AddListener(() => OnOnizlemeButonClick(butonObjeCifti));
            butonObjeCifti.sagadondurbuton.onClick.AddListener(() => Dondur(butonObjeCifti.onizlemeObje, 45));
            butonObjeCifti.soladondurbuton.onClick.AddListener(() => Dondur(butonObjeCifti.onizlemeObje, -45));
            butonObjeCifti.uretbuton.onClick.AddListener(() => CraftObje(butonObjeCifti));

            // Başlangıçta tüm önizleme panellerini kapat
            if (butonObjeCifti.onizlemePanel != null)
            {
                butonObjeCifti.onizlemePanel.SetActive(false);
            }

            GuncelleUretButonDurumu(butonObjeCifti);
            GuncelleMiktarText(butonObjeCifti);
            GuncelleIhtiyacTextleri(butonObjeCifti);
        }
    }


    

    private void Update()
    {
        foreach (var butonObjeCifti in butonObjeCiftleri)
        {
            GuncelleUretButonDurumu(butonObjeCifti);
        }
    }
    

    private void OnOnizlemeButonClick(ButonObjeCifti aktifButonObjeCifti)
    {
        // Diğer tüm önizleme panellerini ve objelerini kapat
        foreach (var butonObjeCifti in butonObjeCiftleri)
        {
            butonObjeCifti.onizlemeObje.SetActive(false);
            SifirlaRotasyon(butonObjeCifti.onizlemeObje);

            if (butonObjeCifti.onizlemePanel != null)
            {
                butonObjeCifti.onizlemePanel.SetActive(false); // Paneli kapat
            }
        }

        // Tıklanan butonun önizleme objesini ve panelini aktif et
        aktifButonObjeCifti.onizlemeObje.SetActive(true);

        if (aktifButonObjeCifti.onizlemePanel != null)
        {
            aktifButonObjeCifti.onizlemePanel.SetActive(true); // Paneli aç
        }
    }



    private void Dondur(GameObject obje, float aci)
    {
        obje.transform.Rotate(Vector3.up, aci);
    }

    private void SifirlaRotasyon(GameObject obje)
    {
        obje.transform.rotation = Quaternion.identity;
    }

    private void CraftObje(ButonObjeCifti butonObjeCifti)
    {
        if (butonObjeCifti.objemiktar < butonObjeCifti.maxobjemiktar)
        {
            // İhtiyaç objelerinin miktarlarını envanterden eksilt
            for (int i = 0; i < butonObjeCifti.ihtiyacobjeisimleri.Length; i++)
            {
                string ihtiyacObjeAdi = butonObjeCifti.ihtiyacobjeisimleri[i];
                int ihtiyacObjeMiktari = butonObjeCifti.ihtiyacobjemiktarlari[i];

                foreach (var item in envanterYonetimi.envanterItemleri)
                {
                    if (item.itemAdi == ihtiyacObjeAdi)
                    {
                        item.itemMiktari -= ihtiyacObjeMiktari;
                        item.itemMiktarText.text = item.itemMiktari.ToString();
                        break;
                    }
                }
            }

            // Objeyi üret
            butonObjeCifti.objemiktar += 1;

            // Uretilecek objeler listesinden aktif olmayan bir objeyi bul ve aktif et
            foreach (var obje in butonObjeCifti.UretilecekObjeler)
            {
                if (!obje.activeInHierarchy)
                {
                    obje.transform.position = butonObjeCifti.onizlemeObje.transform.position;
                    obje.transform.rotation = butonObjeCifti.onizlemeObje.transform.rotation;
                    obje.SetActive(true);
                    break;
                }
            }

            // Önizleme objesini deaktif et ve rotasyonunu sıfırla
            butonObjeCifti.onizlemeObje.SetActive(false);
            SifirlaRotasyon(butonObjeCifti.onizlemeObje);

            // Butonun tıklanabilirliğini güncelle
            GuncelleUretButonDurumu(butonObjeCifti);

            // Üretilecek obje miktarı ve maksimum obje miktarını güncelle
            GuncelleMiktarText(butonObjeCifti);

            // İhtiyaç item isimleri ve miktarlarını güncelle
            GuncelleIhtiyacTextleri(butonObjeCifti);
        }
    }

    private void GuncelleUretButonDurumu(ButonObjeCifti butonObjeCifti)
    {
        // Başlangıçta önizleme butonuna tıklanmadıysa, üret butonunu devre dışı bırak
        if (!butonObjeCifti.onizlemeObje.activeInHierarchy)
        {
            butonObjeCifti.uretbuton.interactable = false;
            return;
        }

        bool ihtiyaclarKarsilandi = true;

        for (int i = 0; i < butonObjeCifti.ihtiyacobjeisimleri.Length; i++)
        {
            string ihtiyacObjeAdi = butonObjeCifti.ihtiyacobjeisimleri[i];
            int ihtiyacObjeMiktari = butonObjeCifti.ihtiyacobjemiktarlari[i];
            bool itemBulundu = false;

            foreach (var item in envanterYonetimi.envanterItemleri)
            {
                if (item.itemAdi == ihtiyacObjeAdi)
                {
                    if (item.itemMiktari < ihtiyacObjeMiktari)
                    {
                        ihtiyaclarKarsilandi = false;
                    }
                    itemBulundu = true;
                    break;
                }
            }

            if (!itemBulundu)
            {
                ihtiyaclarKarsilandi = false;
            }
        }

        bool canInteract = butonObjeCifti.objemiktar < butonObjeCifti.maxobjemiktar &&
                           ihtiyaclarKarsilandi &&
                           (butonObjeCifti.triggerKontrol == null || !butonObjeCifti.triggerKontrol.dokunmaEnabled);

        butonObjeCifti.uretbuton.interactable = canInteract;
    }


    private void GuncelleMiktarText(ButonObjeCifti butonObjeCifti)
    {
        butonObjeCifti.miktarText.text = $"Üretilecek Obje: {butonObjeCifti.objemiktar} / Maks Obje: {butonObjeCifti.maxobjemiktar}";
    }

    private void GuncelleIhtiyacTextleri(ButonObjeCifti butonObjeCifti)
    {
        for (int i = 0; i < butonObjeCifti.ihtiyacobjeisimleri.Length; i++)
        {
            string ihtiyacObjeAdi = butonObjeCifti.ihtiyacobjeisimleri[i];
            int ihtiyacObjeMiktari = butonObjeCifti.ihtiyacobjemiktarlari[i];

            if (i < butonObjeCifti.ihtiyacTextListesi.Count)
            {
                butonObjeCifti.ihtiyacTextListesi[i].text = $"{ihtiyacObjeAdi}: {ihtiyacObjeMiktari}";
            }
        }
    }
}
