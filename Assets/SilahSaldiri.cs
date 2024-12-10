using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilahSaldiri : MonoBehaviour
{
    public Transform hitPozisyon; // Ray'in çıkış noktası
    public Transform mermiAktifPozisyon; // Merminin aktif olacağı pozisyon
    public float rayMesafesi = 100f; // Ray'in maksimum mesafesi
    public List<GameObject> mermiler; // Mermi prefablarını içeren liste
    public float mermiHizi = 10f; // Merminin hareket hızı
    public float mermiAktifSuresi = 2f; // Merminin aktif kalma süresi
    public float yenidenAktifSuresi = 0.5f; // Merminin tekrar kullanılabilir hale gelme süresi
    public float mermiAktifAraligi = 0.5f; // Mermiler arasındaki aktif etme aralığı (saniye)

    private int siradakiMermiIndex = 0; // Sıradaki mermiyi takip eder
    private Vector3 hedefPozisyon; // Merminin gideceği hedef pozisyon
    private bool mermiAktifMi = false; // Mermilerin sırayla aktif olup olmadığı
    public bool atesserbest;
    public string boolisim;
    public Animator anim;
    void FixedUpdate()
    {
        // Ray sürekli çalışır
        RayFirlati();
    }

    void RayFirlati()
    {
        // Ray'i çıkış pozisyonundan ve ileri yönünden başlat
        Ray ray = new Ray(hitPozisyon.position, hitPozisyon.forward);
        RaycastHit hit;

        // Eğer ray bir nesneye çarparsa
        if (Physics.Raycast(ray, out hit, rayMesafesi))
        {
            Debug.Log("Vurulan nesne: " + hit.collider.name);

            // Eğer çarpılan nesnenin adı "Dusman" ise mermiyi aktif et
            if (hit.collider.name == "Dusman" && !mermiAktifMi)
            {
                hedefPozisyon = hit.point; // Çarpılan nesnenin pozisyonunu al
                atesserbest = true;
                if (atesserbest)
                {
                    anim.SetBool(boolisim,true);
                    StartCoroutine(MermiAktifEtSiralama()); // Mermileri sırayla aktif et
                }
              
            }
         
        }
        else
        {
            atesserbest = false;
        }
 

        // Ray'i görselleştirmek için (oyun içinde görünmez)
        Debug.DrawRay(hitPozisyon.position, hitPozisyon.forward * rayMesafesi, Color.red, 0.1f);
    }

    IEnumerator MermiAktifEtSiralama()
    {
        mermiAktifMi = true; // Mermilerin sırayla aktif olduğunu belirle

        // Tüm mermileri sırayla aktif et
        for (int i = 0; i < mermiler.Count; i++)
        {
            GameObject mermi = mermiler[i];
            if (mermi != null && !mermi.activeInHierarchy&& atesserbest)
            {
                mermi.SetActive(true);

                // Mermiyi aktif pozisyonuna taşı
                mermi.transform.position = mermiAktifPozisyon.position;

                // Merminin hedefe doğru hareket etmesini sağla
                StartCoroutine(MermiHareketi(mermi));

                // Mermiler arasında belirli bir aralık bekle
                yield return new WaitForSeconds(mermiAktifAraligi);
            }
        }

        // Bütün mermiler aktif olduktan sonra tekrar aktif etmeye izin ver
        yield return new WaitForSeconds(mermiAktifSuresi);
        mermiAktifMi = false;
    }

    IEnumerator MermiHareketi(GameObject mermi)
    {
        float aktifSure = 0f;

        while (mermi != null && aktifSure < mermiAktifSuresi)
        {
            // Mermiyi hedef pozisyona doğru hareket ettir
            mermi.transform.position = Vector3.MoveTowards(mermi.transform.position, hedefPozisyon, mermiHizi * Time.deltaTime);
            aktifSure += Time.deltaTime;
            yield return null;
        }

        // Mermiyi pasifleştir ve yeniden kullanılabilir hale getirmek için bekle
        if (mermi != null)
        {
            mermi.SetActive(false);
            yield return new WaitForSeconds(yenidenAktifSuresi);
        }
    }
}
