using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KarakterDurum : MonoBehaviour
{
    [Header("Durum Çubukları ve Textleri")]
    public Image aclikBar;        // Açlık durumu için fillAmount
    public TextMeshProUGUI aclikText;  // Açlık için sadece miktar texti

    public Image susuzlukBar;     // Susuzluk durumu için fillAmount
    public TextMeshProUGUI susuzlukText; // Susuzluk için sadece miktar texti

    public Image mutlulukBar;     // Mutluluk durumu için fillAmount
    public TextMeshProUGUI mutlulukText; // Mutluluk için sadece miktar texti

    public Image canBar;          // Can durumu için fillAmount
    public TextMeshProUGUI canText; // Can için sadece miktar texti

    [Header("Durum Değerleri")]
    [Range(0, 1)] public float aclik = 1f;      // Açlık (0-1 arasında)
    [Range(0, 1)] public float susuzluk = 1f;   // Susuzluk (0-1 arasında)
    [Range(0, 1)] public float mutluluk = 1f;   // Mutluluk (0-1 arasında)
    [Range(0, 1)] public float can = 1f;        // Can (0-1 arasında)

    [Header("Durum Azalma Hızı")]
    public float aclikAzalmaHizi = 0.01f;
    public float susuzlukAzalmaHizi = 0.02f;
    public float mutlulukAzalmaHizi = 0.005f;
    public float canAzalmaHizi = 0.03f;
     [Header("Karakter Özellikleri")]
    public float savunma = 1f;
    public float hiz = 1f;

    [Header("Panel Ayarları")]
    public GameObject olunduPanel; // Öldü paneli (aktif-pasif olacak)

    void Start()
    {
        // Başlangıç değerlerini güncelle
        UpdateBarsAndTexts();
    }

    void Update()
    {
        // Durum değerlerini her karede azalt
        aclik = Mathf.Clamp(aclik - aclikAzalmaHizi * Time.deltaTime, 0, 1);
        susuzluk = Mathf.Clamp(susuzluk - susuzlukAzalmaHizi * Time.deltaTime, 0, 1);

        // Açlık ve Susuzluk 50'nin üzerindeyse mutluluk yavaşça artar
        if (aclik > 0.5f && susuzluk > 0.5f)
        {
            mutluluk = Mathf.Clamp(mutluluk + (mutlulukAzalmaHizi * 0.5f) * Time.deltaTime, 0, 1);
        }
        // Açlık veya Susuzluk 30'un altına düşünce mutluluk azalır
        else if (aclik < 0.3f || susuzluk < 0.3f)
        {
            mutluluk = Mathf.Clamp(mutluluk - mutlulukAzalmaHizi * Time.deltaTime, 0, 1);
        }

        // Açlık veya Susuzluk sıfır olduğunda mutluluk ve can azalır
        if (aclik == 0 || susuzluk == 0)
        {
            mutluluk = Mathf.Clamp(mutluluk - mutlulukAzalmaHizi * Time.deltaTime, 0, 1);
            can = Mathf.Clamp(can - canAzalmaHizi * Time.deltaTime, 0, 1);
        }

        // Can sıfır olduğunda öldün panelini aç
        if (can == 0)
        {
            if (olunduPanel != null)
            {
                olunduPanel.SetActive(true);
            }
        }

        // Güncel durumları görsel olarak yansıt
        UpdateBarsAndTexts();
    }

    // FillAmount değerlerini ve TextMeshPro textlerini güncelle
    void UpdateBarsAndTexts()
    {
        if (aclikBar != null) aclikBar.fillAmount = aclik;
        if (aclikText != null) aclikText.text = $"{(int)(aclik * 100)}"; // Sadece sayıyı göster

        if (susuzlukBar != null) susuzlukBar.fillAmount = susuzluk;
        if (susuzlukText != null) susuzlukText.text = $"{(int)(susuzluk * 100)}"; // Sadece sayıyı göster

        if (mutlulukBar != null) mutlulukBar.fillAmount = mutluluk;
        if (mutlulukText != null) mutlulukText.text = $"{(int)(mutluluk * 100)}"; // Sadece sayıyı göster

        if (canBar != null) canBar.fillAmount = can;
        if (canText != null) canText.text = $"{(int)(can * 100)}"; // Sadece sayıyı göster
    }
     public void DegistirSavunma(float miktar)
    {
        savunma = Mathf.Clamp(savunma + miktar, 0, 2f);
    }

    public void DegistirHiz(float miktar)
    {
        hiz = Mathf.Clamp(hiz + miktar, 0.5f, 2f);
    }

    // Açlık durumunu artır veya azalt
    public void DegistirAclik(float miktar)
    {
        aclik = Mathf.Clamp(aclik + miktar, 0, 1);
        UpdateBarsAndTexts();
    }

    // Susuzluk durumunu artır veya azalt
    public void DegistirSusuzluk(float miktar)
    {
        susuzluk = Mathf.Clamp(susuzluk + miktar, 0, 1);
        UpdateBarsAndTexts();
    }

    // Mutluluk durumunu artır veya azalt
    public void DegistirMutluluk(float miktar)
    {
        mutluluk = Mathf.Clamp(mutluluk + miktar, 0, 1);
        UpdateBarsAndTexts();
    }

    // Can durumunu artır veya azalt
    public void DegistirCan(float miktar)
    {
        can = Mathf.Clamp(can + miktar, 0, 1);
        UpdateBarsAndTexts();
    }

}
