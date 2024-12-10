using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaldirVeyaKes : MonoBehaviour
{
    [Header("RayKontrol Referansı")]
    public RayKontrol rayKontrol;

    [Header("Buton Ayarları")]
    public Button saldirButonu; // Saldırı butonu

    [Header("Hasar Değeri")]
    public float hasar = 20f;

    [Header("Animator Listesi")]
    public List<Animator> animators; // Farklı karakterlerin animatörlerinin listesi

    private Animator currentAnimator; // Aktif olan Animator

    private void Start()
    {
        // Butona basıldığında `Saldir` metodunu çalıştır
        if (saldirButonu != null)
        {
            saldirButonu.onClick.AddListener(Saldir);
        }

        // İlk başta aktif olan animatörü belirle (örneğin, ilk animatör)
        SetActiveAnimator();
    }

    private void SetActiveAnimator()
    {
        // Burada aktif karakteri seçmek için bir yol tanımlanabilir
        // Örneğin, en son aktif olan animatör ya da sahnedeki bir karakter.
        // Eğer oyuncu veya karakter bir liste içindeyse, aktif olanı seçebilirsiniz:
        if (animators.Count > 0)
        {
            currentAnimator = animators.Find(anim => anim.gameObject.activeSelf); // Aktif olan animatörü bul
            if (currentAnimator == null)
            {
                Debug.LogWarning("Aktif bir Animator bulunamadı.");
            }
        }
    }

    private void Saldir()
    {
        // RayKontrol'deki hitobje'ye eriş
        if (rayKontrol != null && rayKontrol.hitobje != null)
        {
            // Objenin CanAzalmaVeOlme scriptine ulaş
            CanAzalmaVeOlme canAzalmaVeOlme = rayKontrol.hitobje.GetComponent<CanAzalmaVeOlme>();
            if (canAzalmaVeOlme != null)
            {
                // HasarAl metodunu çağır
                canAzalmaVeOlme.HasarAl(hasar);
            }
            else
            {
                Debug.LogWarning("Hit edilen objede CanAzalmaVeOlme scripti bulunamadı.");
            }

            // Animator'a uygun animasyonu tetikle
            PlayAnimationBasedOnHitObject(rayKontrol.hitobje);

            // Butona tıklandıktan sonra animasyon parametrelerini sıfırla
            StartCoroutine(ResetAllAnimationsWithDelay(1f)); // 1 saniye gecikmeli sıfırlama
        }
        else
        {
            Debug.LogWarning("RayKontrol hitobje null veya RayKontrol referansı atanmadı.");
        }
    }

// Coroutine ile animasyon parametrelerini sıfırlama
    private IEnumerator ResetAllAnimationsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Belirtilen süre kadar bekle
        ResetAllAnimations(); // Animasyonları sıfırla
    }

// Animasyon sıfırlama metodu (örnek)
    private void ResetAllAnimations()
    {
        // Bütün bool parametrelerini false yap
        if (currentAnimator != null)
        {
            currentAnimator.SetBool("isKesme", false);
            currentAnimator.SetBool("isKirma", false);
            currentAnimator.SetBool("isToplama", false);
            currentAnimator.SetBool("isSaldir", false);
        }
    }


    private void PlayAnimationBasedOnHitObject(GameObject hitObject)
    {
        if (currentAnimator == null)
        {
            Debug.LogWarning("Aktif Animator yok!");
            return;
        }

        // Tüm animasyonları false yap
        ResetAllAnimations();

        switch (hitObject.tag)
        {
            case "Agac":
                // Ağaç için kesme animasyonunu oynat
                currentAnimator.SetBool("isKesme", true);
                break;
            case "Tas":
                // Taş için kırma animasyonunu oynat
                currentAnimator.SetBool("isKirma", true);
                break;
            case "Meyve":
                // Meyve için toplama animasyonunu oynat
                currentAnimator.SetBool("isToplama", true);
                break;
            case "Enemy":
                // Düşman için saldırma animasyonunu oynat
                currentAnimator.SetBool("isSaldir", true);
                break;
            default:
                Debug.LogWarning("Tanımlanamayan hitobje: " + hitObject.tag);
                break;
        }
    }

}
