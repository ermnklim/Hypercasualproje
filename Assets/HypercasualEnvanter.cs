using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro namespace

public class HypercasualEnvanter : MonoBehaviour
{
    // Kaynak türleri için enum
    public enum EnvanterTurleri
    {
        Altin,
        Tas,
        Agac,
        Et,
        Meyve
    }

    // Kaynak bilgisi için sınıf
    [System.Serializable]
    public class EnvanterObjesi
    {
        public EnvanterTurleri envanterTuru;    // Kaynağın türü
        public int miktar;                      // Kaynağın miktarı
        public Image envanterResmi;             // Kaynağın UI görseli
        public TextMeshProUGUI envanterMiktarText; // Kaynağın miktarını gösteren TextMeshPro
    }

    // Envanterdeki kaynakların listesi
    [SerializeField] private List<EnvanterObjesi> envanterObjeleri = new List<EnvanterObjesi>();

    // Başlangıçta her şeyin görünümünü günceller
    void Start()
    {
        EnvanterGorunumuGuncelle();
    }

    // Envanterdeki kaynakların miktarını UI'ye göre günceller
    void EnvanterGorunumuGuncelle()
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            // Görselleri güncelle
            if (obje.miktar > 0)
            {
                obje.envanterResmi.gameObject.SetActive(true);  // Resmi aktif et
            }
            else
            {
                obje.envanterResmi.gameObject.SetActive(false); // Resmi deaktif et
            }

            // TextMeshPro miktarını güncelle
            if (obje.envanterMiktarText != null)
            {
                obje.envanterMiktarText.text = obje.miktar.ToString();  // Miktarı yazdır
            }
        }
    }

    // Miktarını arttıran fonksiyon
    public void MiktarArttir(EnvanterTurleri tur, int miktar)
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.envanterTuru == tur)
            {
                obje.miktar += miktar;
                break;
            }
        }
        EnvanterGorunumuGuncelle();  // UI güncelle
    }

    // Miktarını azaltan fonksiyon
    public void MiktarAzalt(EnvanterTurleri tur, int miktar)
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.envanterTuru == tur)
            {
                obje.miktar = Mathf.Max(0, obje.miktar - miktar);  // Miktar 0'dan küçük olmasın
                break;
            }
        }
        EnvanterGorunumuGuncelle();  // UI güncelle
    }

    // Miktarı manuel olarak ayarlamak için fonksiyon
    public void MiktarGuncelle(EnvanterTurleri tur, int yeniMiktar)
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.envanterTuru == tur)
            {
                obje.miktar = yeniMiktar;
                break;
            }
        }
        EnvanterGorunumuGuncelle();  // UI güncelle
    }

    // Envanterdeki kaynağın miktarını almak için fonksiyon
    public int KaynakMiktariniAl(EnvanterTurleri tur)
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.envanterTuru == tur)
            {
                return obje.miktar;
            }
        }
        return 0; // Kaynak bulunamadıysa 0 döndür
    }
}
