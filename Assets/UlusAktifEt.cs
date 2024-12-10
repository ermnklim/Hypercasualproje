using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjeYerlestirici : MonoBehaviour
{
    public enum Kategori
    {
        Altinlar,
        Taslar,
        Agaclar,
        Hayvanlar,
        Meyveler,
        Uluslar
    }

    [System.Serializable]
    public class KategoriBilgisi
    {
        public Kategori kategori;
        public List<GameObject> uretilecekObjeler;
        public float minimumMesafe;
    }

    [SerializeField] private List<KategoriBilgisi> kategorilerBilgisi = new List<KategoriBilgisi>();
    [SerializeField] private Transform plane;

    // Uluslar için önceden tanımlanmış pozisyonlar
    [SerializeField] private List<Transform> uluslarPozisyonlari = new List<Transform>();

    private List<Vector3> kullanilanPozisyonlar = new List<Vector3>();
    private List<Transform> kullanilmayanUlusPozisyonlari;

    private const int MAKS_DENEME = 100;
    private const float ULUS_MESAFE_CARPANI = 5.5f; // Uluslara olan mesafeyi artırmak için çarpan

    void Awake()
    {
        if (plane == null)
        {
            plane = GameObject.FindGameObjectWithTag("Plane").transform;
        }

        // Kullanılabilir ulus pozisyonlarını listeye aktar
        kullanilmayanUlusPozisyonlari = new List<Transform>(uluslarPozisyonlari);

        KategorileriYerlestir();
    }

    void KategorileriYerlestir()
    {
        foreach (KategoriBilgisi kategoriBilgisi in kategorilerBilgisi)
        {
            foreach (GameObject obje in kategoriBilgisi.uretilecekObjeler)
            {
                if (kategoriBilgisi.kategori == Kategori.Uluslar)
                {
                    UluslarIcinPozisyonAyarla(obje, kategoriBilgisi.minimumMesafe);
                }
                else
                {
                    YerlesimDenemesi(obje, kategoriBilgisi.minimumMesafe);
                }
            }
        }
    }

    void UluslarIcinPozisyonAyarla(GameObject obje, float minimumMesafe)
    {
        if (kullanilmayanUlusPozisyonlari.Count > 0)
        {
            int randomIndex = Random.Range(0, kullanilmayanUlusPozisyonlari.Count);
            Transform secilenPozisyon = kullanilmayanUlusPozisyonlari[randomIndex];

            // Uluslar pozisyonunu mesafe kontrolü yapılmadan kullanıyoruz
            obje.transform.position = secilenPozisyon.position;
            obje.SetActive(true);

            // Kullanılan pozisyonları listeye ekle
            kullanilanPozisyonlar.Add(secilenPozisyon.position);

            // Pozisyonu kullanılmayanlar listesinden çıkar
            kullanilmayanUlusPozisyonlari.RemoveAt(randomIndex);
        }
        else
        {
            Debug.LogWarning("Tüm ulus pozisyonları kullanıldı!");
        }
    }

    void YerlesimDenemesi(GameObject obje, float minimumMesafe)
    {
        bool yerlesimBasarili = false;
        int denemeSayisi = 0;

        while (!yerlesimBasarili && denemeSayisi < MAKS_DENEME)
        {
            Vector3 yeniPozisyon = RandomGuvenliBolgeAl(minimumMesafe);
            if (yeniPozisyon != Vector3.zero)
            {
                obje.transform.position = yeniPozisyon;
                obje.SetActive(true);
                kullanilanPozisyonlar.Add(yeniPozisyon);
                yerlesimBasarili = true;
            }
            denemeSayisi++;
        }

        if (!yerlesimBasarili)
        {
            Debug.LogWarning($"{obje.name} için uygun bir pozisyon bulunamadı, varsayılan pozisyona yerleştiriliyor.");
            Vector3 fallbackPozisyon = plane.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            obje.transform.position = fallbackPozisyon;
            obje.SetActive(true);
            kullanilanPozisyonlar.Add(fallbackPozisyon);
        }
    }

    Vector3 RandomGuvenliBolgeAl(float minimumMesafe)
    {
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        float planeWidth = planeRenderer.bounds.size.x;
        float planeLength = planeRenderer.bounds.size.z;

        for (int i = 0; i < MAKS_DENEME; i++)
        {
            float randomX = Random.Range(-planeWidth / 2f + 1f, planeWidth / 2f - 1f);
            float randomZ = Random.Range(-planeLength / 2f + 1f, planeLength / 2f - 1f);
            Vector3 randomPozisyon = new Vector3(randomX, 0, randomZ) + plane.position;

            if (MesafeKontrolu(randomPozisyon, minimumMesafe))
            {
                return randomPozisyon;
            }
        }

        return Vector3.zero;
    }

    bool MesafeKontrolu(Vector3 pozisyon, float minimumMesafe)
    {
        foreach (Vector3 kullanilanPozisyon in kullanilanPozisyonlar)
        {
            float aktifMesafe = minimumMesafe;

            // Eğer pozisyon bir ulusun konumuysa mesafeyi artır
            if (IsUlusPozisyonu(kullanilanPozisyon))
            {
                aktifMesafe *= ULUS_MESAFE_CARPANI;
            }

            if (Vector3.Distance(pozisyon, kullanilanPozisyon) < aktifMesafe)
            {
                return false;
            }
        }
        return true;
    }

    bool IsUlusPozisyonu(Vector3 pozisyon)
    {
        foreach (Transform ulusPozisyon in uluslarPozisyonlari)
        {
            if (Vector3.Distance(pozisyon, ulusPozisyon.position) < 0.1f) // Eşitlik kontrolü için tolerans
            {
                return true;
            }
        }
        return false;
    }
}
