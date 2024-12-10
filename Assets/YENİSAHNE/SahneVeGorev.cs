using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ZorlukAyari
{
    public string zorlukAdi;
    public float zorlukCarpani;
    public Color zorlukRengi;
    public bool aktif;
}

[System.Serializable]
public class Gorev
{
    public string gorevAdi;
    public int gorevMiktari;
    public bool tamamlandi;
    public Image gorevresim;
    public TextMeshProUGUI gorevtext;
}

[System.Serializable]
public class Sahne
{
    public string sahneAdi;
    public int sahneSirasi;
    public bool aktif;
}

public class SahneVeGorev : MonoBehaviour
{
    [SerializeField]
    private List<Gorev> gorevler = new List<Gorev>();
    
    [SerializeField]
    private List<Sahne> sahneler = new List<Sahne>();
    
    [SerializeField]
    private List<ZorlukAyari> zorlukSeviyeleri = new List<ZorlukAyari>();

    private ZorlukAyari aktifZorluk;

    void Start()
    {
        // Başlangıçta aktif zorluk seviyesini bul
        aktifZorluk = zorlukSeviyeleri.Find(z => z.aktif);
        if (aktifZorluk != null)
        {
            Debug.Log($"Aktif zorluk seviyesi: {aktifZorluk.zorlukAdi}");
        }
    }

    // Zorluk seviyesini değiştirmek için method
    public void ZorlukSeviyesiDegistir(string zorlukAdi)
    {
        var yeniZorluk = zorlukSeviyeleri.Find(z => z.zorlukAdi == zorlukAdi);
        if (yeniZorluk != null)
        {
            // Önceki aktif zorluğu devre dışı bırak
            if (aktifZorluk != null)
                aktifZorluk.aktif = false;

            // Yeni zorluğu aktif et
            yeniZorluk.aktif = true;
            aktifZorluk = yeniZorluk;
            
            Debug.Log($"Zorluk seviyesi değiştirildi: {zorlukAdi}");
            Debug.Log($"Yeni zorluk çarpanı: {aktifZorluk.zorlukCarpani}");
        }
    }

    // Görev zorluğunu hesaplamak için örnek method
    public float GorevZorluguHesapla(int baseMiktar)
    {
        if (aktifZorluk != null)
        {
            return baseMiktar * aktifZorluk.zorlukCarpani;
        }
        return baseMiktar;
    }

    // Görevleri kontrol etmek için örnek method
    public void GorevGuncelle(string gorevAdi, int tamamlananMiktar)
    {
        var gorev = gorevler.Find(g => g.gorevAdi == gorevAdi);
        if (gorev != null)
        {
            if (tamamlananMiktar >= gorev.gorevMiktari)
            {
                gorev.tamamlandi = true;
                Debug.Log($"{gorevAdi} görevi tamamlandı!");
            }
        }
    }

    // Sahne durumunu kontrol etmek için örnek method
    public void SahneyeGec(string sahneAdi)
    {
        var sahne = sahneler.Find(s => s.sahneAdi == sahneAdi);
        if (sahne != null && sahne.aktif)
        {
            Debug.Log($"{sahneAdi} sahnesine geçiliyor...");
            // Burada sahne geçiş kodları yazılabilir
        }
    }
}