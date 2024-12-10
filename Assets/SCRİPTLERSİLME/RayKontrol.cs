using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayKontrol : MonoBehaviour
{
    [Header("Işın Ayarları")]
    public Transform kafaPozisyonu;    // Işının fırlatılacağı konum
    public float isinMesafesi = 5f;    // Işının maksimum mesafesi
    
    [Header("Arayüz Butonları")]
    public Button agacKesmeButonu;     // Ağaç kesme butonu
    public Button tasKirmaButonu;      // Taş kırma butonu
    public Button saldiriButonu;       // Düşmana saldırma butonu
    public Button meyveToplamaButonu;  // Meyve toplama butonu
    public GameObject hitobje;

    [Header("Envanter")]
    public List<GameObject> baltaListesi; // Liste, aktif balta objeleri
    public List<GameObject> kazmaListesi; // Liste, aktif kazma objeleri
    public List<GameObject> silahListesi; // Liste, aktif silah objeleri
    public List<GameObject> bıçakListesi; // Liste, aktif bıçak objeleri

    private void Start()
    {
        ButonlariKapat();
    }

    private void Update()
    {
        RaycastHit carpma;
        Ray isin = new Ray(kafaPozisyonu.position, kafaPozisyonu.forward);
        Debug.DrawRay(isin.origin, isin.direction * isinMesafesi, Color.red);

        if (Physics.Raycast(isin, out carpma, isinMesafesi))
        {
            switch (carpma.collider.tag)
            {
                case "Agac":
                    if (baltaListesi.Exists(item => item.activeSelf)) // Eğer balta aktifse
                    {
                        ButonuAktifEt(agacKesmeButonu);
                    }
                    else
                    {
                        ButonuPasifEt(agacKesmeButonu);
                    }
                    hitobje = carpma.transform.gameObject;
                    break;
                case "Tas":
                    if (kazmaListesi.Exists(item => item.activeSelf)) // Eğer kazma aktifse
                    {
                        ButonuAktifEt(tasKirmaButonu);
                    }
                    else
                    {
                        ButonuPasifEt(tasKirmaButonu);
                    }
                    hitobje = carpma.transform.gameObject;
                    break;
                case "Enemy":
                    if (silahListesi.Exists(item => item.activeSelf)) // Eğer silah aktifse
                    {
                        ButonuAktifEt(saldiriButonu);
                    }
                    else
                    {
                        ButonuPasifEt(saldiriButonu);
                    }
                    hitobje = carpma.transform.gameObject;
                    break;
                case "Meyve":
                    if (bıçakListesi.Exists(item => item.activeSelf)) // Eğer bıçak aktifse
                    {
                        ButonuAktifEt(meyveToplamaButonu);
                    }
                    else
                    {
                        ButonuPasifEt(meyveToplamaButonu);
                    }
                    hitobje = carpma.transform.gameObject;
                    break;
                default:
                    ButonlariKapat();
                    hitobje = null;
                    break;
            }
        }
        else
        {
            ButonlariKapat();
        }
    }

    private void ButonuAktifEt(Button aktifEdilecekButon)
    {
        ButonlariKapat();
        if (aktifEdilecekButon != null)
        {
            aktifEdilecekButon.gameObject.SetActive(true);
        }
    }

    private void ButonuPasifEt(Button pasifEdilecekButon)
    {
        if (pasifEdilecekButon != null)
        {
            pasifEdilecekButon.gameObject.SetActive(false);
        }
    }

    private void ButonlariKapat()
    {
        if (agacKesmeButonu != null) agacKesmeButonu.gameObject.SetActive(false);
        if (tasKirmaButonu != null) tasKirmaButonu.gameObject.SetActive(false);
        if (saldiriButonu != null) saldiriButonu.gameObject.SetActive(false);
        if (meyveToplamaButonu != null) meyveToplamaButonu.gameObject.SetActive(false);
    }
}
