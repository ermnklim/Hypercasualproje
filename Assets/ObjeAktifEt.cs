using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjeAktifEt : MonoBehaviour
{
    public enum ObjeTuru
    {
        Altin,
        Agac,
        Tas,
        Meyve
    }

    [System.Serializable]
    public struct ObjeTuruBilgisi
    {
        public ObjeTuru objeTuru;  // Enum türü
        public List<GameObject> objeler;    // Objeler listesi
        public int aktifEdilecekSayi;  // Bu objelerden ne kadar aktif edilecek
        public float minimumMesafe;  // Bu obje türü için minimum mesafe
        public float aktifOlmaSuresi; // Objelerin aktif olacağı süre
    }

    [SerializeField] private List<ObjeTuruBilgisi> objelerBilgileri = new List<ObjeTuruBilgisi>();  // Objeler ve aktif etme sayıları
    [SerializeField] private Transform plane; // Referans plane

    private List<Vector3> kullanilanPozisyonlar = new List<Vector3>();  // Kullanılan pozisyonlar
    private List<GameObject> deaktiveObjeler = new List<GameObject>();  // Deaktive olan objeler

    void Start()
    {
        if (plane == null)
        {
            plane = GameObject.FindGameObjectWithTag("Plane").transform;
        }

        ObjeYerlesimiYap();
    }

    void ObjeYerlesimiYap()
    {
        // Objeler yerleşimi yapılırken her obje türü için
        foreach (var objeBilgisi in objelerBilgileri)
        {
            int aktifEdilenObjeSayisi = 0; // Aktif edilen objeleri say

            // Belirtilen aktif edilme sayısı kadar objeyi yerleştir
            while (aktifEdilenObjeSayisi < objeBilgisi.aktifEdilecekSayi)
            {
                // Eğer objeler listesinde herhangi bir obje aktif edilecekse
                if (objeBilgisi.objeler.Count > 0)
                {
                    // Her bir objeden birini rastgele seç
                    GameObject secilenObje = objeBilgisi.objeler[Random.Range(0, objeBilgisi.objeler.Count)];

                    // Eğer obje zaten aktifse, bunu atla
                    if (secilenObje.activeInHierarchy)
                    {
                        continue;
                    }

                    // Uygun pozisyonu al
                    Vector3 yeniPozisyon = RandomGuvenliBolgeAl(objeBilgisi.minimumMesafe);

                    // Eğer uygun pozisyon bulunduysa
                    if (yeniPozisyon != Vector3.zero)
                    {
                        secilenObje.SetActive(true); // Obje aktif hale getiriliyor
                        secilenObje.transform.position = yeniPozisyon; // Objenin yeni pozisyonu
                        kullanilanPozisyonlar.Add(yeniPozisyon); // Kullanılan pozisyonları kaydet
                        aktifEdilenObjeSayisi++; // Aktif edilen objeler sayısını artır

                        // Add to the list of deactivated objects to be reactivated later
                        deaktiveObjeler.Add(secilenObje);

                        Debug.Log($"{secilenObje.name} objesi aktif edildi. Toplam aktif edilen: {aktifEdilenObjeSayisi}");
                    }
                    else
                    {
                        // Eğer uygun pozisyon bulunamazsa, objeyi aktif etmeyi dene
                        Debug.LogWarning("Yeterli alan bulunamadı, başka bir obje aktif edilecek.");
                    }
                }
                else
                {
                    Debug.LogWarning("Aktif edilecek obje listesi boş!");
                    break;
                }
            }
        }

        // Start coroutine to reactivate objects after their active time has passed
        StartCoroutine(ReactivateObjects());
    }

    Vector3 RandomGuvenliBolgeAl(float minimumMesafe)
    {
        // Plane'in boyutlarını al
        Vector3 planeScale = plane.localScale;
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        float planeWidth = planeRenderer.bounds.size.x;
        float planeLength = planeRenderer.bounds.size.z;

        // Pozisyonu oluşturmak için pozisyon kontrolü yapılacak
        Vector3 randomPozisyon = Vector3.zero;
        bool uygunPozisyonBulundu = false;

        // Sonsuz bir döngü ile uygun pozisyon aranacak
        while (!uygunPozisyonBulundu)
        {
            // Random pozisyon oluştur
            float randomX = Random.Range(-planeWidth / 2f + 1f, planeWidth / 2f - 1f);
            float randomZ = Random.Range(-planeLength / 2f + 1f, planeLength / 2f - 1f);
            randomPozisyon = new Vector3(randomX, 0, randomZ) + plane.position;

            // Objeler arasındaki mesafeyi kontrol et ve çakışma olup olmadığını kontrol et
            bool mesafeUygun = true;
            foreach (Vector3 kullanilanPozisyon in kullanilanPozisyonlar)
            {
                if (Vector3.Distance(randomPozisyon, kullanilanPozisyon) < minimumMesafe)
                {
                    mesafeUygun = false;
                    break;
                }
            }

            // Eğer mesafe uygunsa, pozisyon uygun demektir.
            if (mesafeUygun)
            {
                uygunPozisyonBulundu = true; // Pozisyon bulundu
            }
        }

        return randomPozisyon;
    }

    IEnumerator ReactivateObjects()
    {
        // Go through the list of deactivated objects and reactivate them
        foreach (var objeBilgisi in objelerBilgileri)
        {
            foreach (var obje in deaktiveObjeler)
            {
                if (!obje.activeInHierarchy) // Only reactivate if the object is currently inactive
                {
                    // Wait for the specified "active time"
                    yield return new WaitForSeconds(objeBilgisi.aktifOlmaSuresi);

                    // Reactivate the object at a new random position
                    Vector3 yeniPozisyon = RandomGuvenliBolgeAl(0); // You can pass different minimum distance if needed
                    obje.transform.position = yeniPozisyon;
                    obje.SetActive(true);

                    // Add the new position to the used positions list
                    kullanilanPozisyonlar.Add(yeniPozisyon);

                    Debug.Log($"{obje.name} objesi yeniden aktif oldu.");
                }
            }
        }
    }
}
