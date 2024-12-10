using System;
using UnityEngine;
using TMPro; // TextMeshPro'yu kullanmak için gerekli
using Random = UnityEngine.Random;

public class Can : MonoBehaviour
{
    public float mycan, maxcan;
    public float minTimeToReactivate = 2f;
    public float maxTimeToReactivate = 5f;
    private bool isDeactivating = false;
    public bool small;

    public HypercasualEnvanter envanter; // Envanter referansı
    public HypercasualEnvanter.EnvanterTurleri envanterTuru; // Çarpışma sırasında hangi tür envanter artırılacak

    public TextMeshProUGUI envanterText; // Envanteri ekranda gösterecek TextMeshPro

    void Start()
    {
        mycan = maxcan; // Başlangıç değeri
        UpdateUI(); // Başlangıçta UI'yi güncelle
    }

    void Update()
    {
        if (mycan <= 0)
        {
            mycan = 0;
            if (!isDeactivating) // Şu anda devre dışı bırakılmıyorsa
            {
                isDeactivating = true;
                Invoke("ReactivateCan", Random.Range(minTimeToReactivate, maxTimeToReactivate));
                gameObject.SetActive(false); // Nesneyi devre dışı bırak
            }
        }

        UpdateUI(); // UI'yi her frame'de güncelle
    }

    private void ReactivateCan()
    {
        mycan = maxcan; // Yeniden etkinleştirildiğinde canı sıfırla
        isDeactivating = false; // Devre dışı bırakılma durumunu sıfırla
        gameObject.SetActive(true);
    }

    public void IncreaseCan(float amount)
    {
        mycan += amount;
        if (mycan > maxcan) mycan = maxcan; // Maksimum canı aşmasını engelle
        UpdateUI(); // UI'yi güncelle
    }

    public void DecreaseCan(float amount)
    {
        mycan -= amount;
        if (mycan < 0) mycan = 0; // Minimum canın altına düşmesini engelle
        UpdateUI(); // UI'yi güncelle
    }

    // UI güncellemesi için yardımcı fonksiyon
    private void UpdateUI()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (small)
            {
                // Envanter miktarını artır
                if (envanter != null)
                {
                    envanter.MiktarArttir(envanterTuru, 1);
                    if (envanterText != null && envanter != null)
                    {
                        int envanterMiktari = envanter.KaynakMiktariniAl(envanterTuru); // Envanterdeki miktarı al
                        envanterText.text = $"{envanterTuru.ToString()}: {envanterMiktari}"; // Kaynak türünü ve miktarını göster
                    }
                    gameObject.SetActive(false); // Nesneyi devre dışı bırak
                   Invoke("disabletext",1f);
                }
            }
        }
    }

    private void disabletext()
    {
        envanterText.text = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (small)
            {
                // Envanter miktarını artır
                if (envanter != null)
                {
                    envanter.MiktarArttir(envanterTuru, 1);
                    if (envanterText != null && envanter != null)
                    {
                        int envanterMiktari = envanter.KaynakMiktariniAl(envanterTuru); // Envanterdeki miktarı al
                        envanterText.text = $"{envanterTuru.ToString()}: {envanterMiktari}"; // Kaynak türünü ve miktarını göster
                    }
                    gameObject.SetActive(false); // Nesneyi devre dışı bırak
                    Invoke("disabletext",.3f);
                }
            }
        }
    }
}
