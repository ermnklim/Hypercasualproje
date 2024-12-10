using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Konusma : MonoBehaviour
{
    [System.Serializable]
    public class Konusmaci
    {
        public string isim; // Konuşmacının ismi
        public Sprite resim; // Konuşmacının resmi
        public string konusma; // Konuşmacının konuşması
    }

    public Konusmaci[] konusmacilar; // Tüm konuşmacılar
    public int aktifKonusma = 0; // Aktif konuşma indexi

    public TextMeshProUGUI konusmatext; // Konuşma metni
    public TextMeshProUGUI isimtext; // Konuşmacı ismi
    public Image konusmaciImage; // Konuşmacı resmi
    public Button ilerlebuton; // İlerle butonu
    public GameObject konusmapanel; // Konuşma paneli
    public bool sahnedegistir;
    public string scenename;

    private void Start()
    {
        // Eğer konuşmacı dizisi boş değilse, ilk konuşmayı başlat
        if (konusmacilar.Length > 0)
        {
            GuncellePanel();
        }

        // Buton tıklama olayını bağla
        ilerlebuton.onClick.AddListener(Ilerle);
    }

    public void Ilerle()
    {
        // Eğer son konuşmaya gelindiyse, paneli kapat
        if (aktifKonusma == konusmacilar.Length - 1)
        {
            if (sahnedegistir)
            {
                SceneManager.LoadScene(scenename);
            }
            else
            {
                konusmapanel.SetActive(false);
            }
            
            return;
        }

        // Konuşmayı ilerlet
        aktifKonusma++;
        GuncellePanel();
    }

    private void GuncellePanel()
    {
        // Aktif konuşmacıyı al
        Konusmaci aktifKonusmaci = konusmacilar[aktifKonusma];

        // Panelde bilgileri güncelle
        konusmatext.text = aktifKonusmaci.konusma;
        isimtext.text = aktifKonusmaci.isim;
        konusmaciImage.sprite = aktifKonusmaci.resim;
    }
}