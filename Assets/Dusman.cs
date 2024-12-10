using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Düşman türleri (Zombi, Canavar, Insan)
public enum DusmanTipi
{
    Zombi,
    Canavar,
    Insan
}

// Saldırı türleri (KısaMesafe, UzakMesafe, Alevli)
public enum SaldiriTipi
{
    KisaMesafe,
    UzakMesafe,
    Alevli
}

public class Dusman : MonoBehaviour
{
    // Enums'ları Unity Editörü'nde görünür hale getirmek
    public DusmanTipi dusmanTipi;
    public SaldiriTipi saldiriTipi;

    // Mermi listesi ve hız ayarları
    public List<GameObject> bullets; // Mermi prefabları
    public float bulletSpeed = 10f;  // Merminin hızı
    public Transform shootPoint;  // Mermilerin ateş edileceği nokta

    public Transform player;  // Oyuncu referansı
    public float attackRange = 5f;  // Saldırı mesafesi
    public float detectionRange = 10f;  // Farketme mesafesi
    public float attackCooldown = 1f;  // Saldırı arasındaki bekleme süresi
    public float can = 100f;  // Düşman canı (0 ile 100 arasında)

    private bool canAttack = true;  // Saldırı yapılıp yapılmayacağını kontrol eden flag
    private bool isPlayerInRange = false;  // Oyuncu yakınsa saldırı yapılacak
    private List<GameObject> activeBullets = new List<GameObject>();  // Aktif olan mermiler
    private bool isDead = false;  // Düşman öldü mü?
    public float respawnDelay = 5f;  // Ölme sonrası tekrar aktif olma süresi
    public float respawnDistance = 10f;  // Ölme sonrası belirleyeceğiniz mesafe

    // Başlangıçta yapılacak işlemler
    void Start()
    {
        // Player tag'ine göre oyuncu bulunur
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;  // Oyuncu tag'ini kullanarak bulma
        }

        // Önceden sahnede yer alan tüm mermiler set edilmelidir
        foreach (GameObject bullet in bullets)
        {
            bullet.SetActive(false);  // Başlangıçta tüm mermiler kapalı olacak
        }
    }

    // Update her frame'de çalışır
    void Update()
    {
        // Eğer öldüyse, yeniden doğmayı bekle
        if (isDead) return;

        // Oyuncu ile olan mesafeyi hesapla
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Eğer oyuncu farketme mesafesinde ise, düşman saldırıya geçebilir
        if (distanceToPlayer <= detectionRange)
        {
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }

        // Eğer oyuncu saldırı mesafesindeyse ve düşman saldırabiliyorsa, saldırıyı başlat
        if (isPlayerInRange)
        {
            // Oyuncuya dön
            FacePlayer();

            if (distanceToPlayer <= attackRange && canAttack)
            {
                Attack();
            }
        }
        else
        {
            // Oyuncu menzilden çıktığında saldırıyı durdur
            if (!isPlayerInRange)
            {
                StopAttack();
            }
        }
    }
    
    // Düşman çarptığında canını azaltan metot
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Mermi"))  // Eğer çarpan obje mermi ise
        {
            // Mermiden can azalt
            //ReduceHealth(10f);  // Örnek olarak 10 can kaybediyor
        }
    }

    // Can azaltma
    public void ReduceHealth(float damage)
    {
        can -= damage;
        if (can <= 0f && !isDead)
        {
            Die();
        }
    }

    // Düşman öldüğünde yapılacak işlemler
    void Die()
    {
        isDead = true;
        Debug.Log("Düşman öldü!");
        gameObject.SetActive(false);  // Düşmanı deaktif et

        // Belirli bir süre sonra tekrar aktif et
        StartCoroutine(Respawn());
    }

    // Düşman öldükten sonra tekrar aktif olma
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);  // Belirtilen süre kadar bekle

        // Düşmanı tekrar aktif et
        transform.position = player.position + new Vector3(Random.Range(-respawnDistance, respawnDistance), 0f, Random.Range(-respawnDistance, respawnDistance));  // Oyuncudan uzak mesafede yeniden doğ
        gameObject.SetActive(true);

        // Canı yeniden doldur
        can = 100f;
        isDead = false;
        Debug.Log("Düşman yeniden doğdu!");
    }

    // Saldırıyı başlat
    void Attack()
    {
        // Saldırı bekleme süresi
        StartCoroutine(AttackCooldown());

        // Seçilen saldırı türüne göre saldırıyı başlat
        if (saldiriTipi == SaldiriTipi.KisaMesafe)
        {
            SaldirKisaMesafe();
        }
        else if (saldiriTipi == SaldiriTipi.UzakMesafe)
        {
            SaldirUzakMesafe();
        }
        else if (saldiriTipi == SaldiriTipi.Alevli)
        {
            SaldirAlevli();
        }
    }

    // Kısa mesafeli saldırı (melee)
    void SaldirKisaMesafe()
    {
        // Burada yakın mesafede saldırı yapılabilir
        Debug.Log("Kısa Mesafe Saldırısı!");
    }

    // Uzak mesafeli saldırı (range)
    void SaldirUzakMesafe()
    {
        // Uzak mesafe saldırısı için bir mermi aktif et
        GameObject bullet = GetInactiveBullet();
        if (bullet != null)
        {
            bullet.transform.position = shootPoint.position;  // Mermiyi doğru noktaya yerleştir
            bullet.transform.rotation = shootPoint.rotation;
            bullet.SetActive(true);  // Mermiyi aktif et
            // 3 saniye sonra mermiyi deaktif et
            StartCoroutine(DeactivateBullet(bullet, 3f));
        }
        else
        {
            Debug.Log("Aktif mermi yok!");
        }
    }

    // Alevli saldırı
    void SaldirAlevli()
    {
        // Burada alevli bir saldırı yapılabilir
        Debug.Log("Alevli Saldırı!");
    }

    // Saldırı cooldown'ı
    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Saldırıyı durdur
    void StopAttack()
    {
        Debug.Log("Oyuncu menzilden çıktı, saldırı durduruluyor.");
    }

    // Oyuncuya dönme
    void FacePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;  // Y eksenini sıfırlayarak sadece yatay düzlemde hareket

        // Düşmanı oyuncuya döndürme
        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);
        }
    }

    // Aktif olmayan bir mermiyi bulma
    GameObject GetInactiveBullet()
    {
        foreach (GameObject bullet in bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;  // Bulunan ilk deaktif mermiyi döndür
            }
        }
        return null;  // Eğer hiç deaktif mermi yoksa, null döndür
    }

    // Mermiyi belirli bir süre sonra deaktif yapma
    IEnumerator DeactivateBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false);  // 3 saniye sonra mermiyi deaktif yap
    }
}
