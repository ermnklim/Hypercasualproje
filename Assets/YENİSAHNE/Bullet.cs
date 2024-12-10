using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Enum tipi ile silah türlerini belirliyoruz
    public enum SilahTipi
    {
        Tabanca,
        Pompalı,
        KeskinNisanci,
        Tüfek  // Yeni silah tipi ekledik
    }

    public SilahTipi silahTipi; // Silah tipi seçimi (Editör üzerinden)
    
    public float hiz; // Merminin hareket hızı
    public float yasamSuresi; // Merminin maksimum yaşam süresi
    public int hasar; // Merminin verdiği hasar

    private Rigidbody rb; // Merminin fiziksel hareketini kontrol etmek için Rigidbody
    private float yasamSayaci; // Yaşam süresini takip eder

    // Merminin özelliklerini silah tipine göre ayarlayacağız
    void Start()
    {
        SetGunProperties();
    }

    // Silah tipine göre merminin özelliklerini ayarlayan fonksiyon
    void SetGunProperties()
    {
        // Silah türüne göre farklı özellikler
        switch (silahTipi)
        {
            case SilahTipi.Tabanca:
                hiz = 20f; // Tabanca hızı
                yasamSuresi = 3f; // Tabanca yaşam süresi
                hasar = 10; // Tabanca hasar
                break;
                
            case SilahTipi.Pompalı:
                hiz = 10f; // Pompalı tüfek hızı
                yasamSuresi = 5f; // Pompalı yaşam süresi
                hasar = 20; // Pompalı hasar
                break;
                
            case SilahTipi.KeskinNisanci:
                hiz = 50f; // Keskin nişancı hızı
                yasamSuresi = 7f; // Keskin nişancı yaşam süresi
                hasar = 50; // Keskin nişancı hasar
                break;
                
            case SilahTipi.Tüfek:
                hiz = 30f; // Tüfek hızı
                yasamSuresi = 6f; // Tüfek yaşam süresi
                hasar = 25; // Tüfek hasar
                break;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // Mermiyi başlangıç pozisyonuna getir
        if (rb != null)
            rb.velocity = transform.forward * hiz;

        // Yaşam süresini sıfırla
        yasamSayaci = 0f;
    }

    void Update()
    {
        // Yaşam süresini kontrol et
        yasamSayaci += Time.deltaTime;
        if (yasamSayaci >= yasamSuresi)
        {
            // Mermiyi devre dışı bırak
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dusman"))
        {
            Dusman dusman = collision.gameObject.GetComponent<Dusman>(); // Düşman scriptini al
            if (dusman != null)
            {
                dusman.ReduceHealth(hasar);  // Düşmanın canını azalt
            }
            gameObject.SetActive(false); // Mermiyi deaktif et
        }
    }
}
