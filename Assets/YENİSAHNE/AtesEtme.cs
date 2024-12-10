using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtesEtme : MonoBehaviour
{
    public enum SilahTuru
    {
        Tabanca,
        Tufek,
        Pompalı,
        KeskinNisanci
    }

    [System.Serializable]
    public class Silah
    {
        public SilahTuru silahTuru;
        public GameObject silahObjesi;
        public Button atesButonu;
        public List<GameObject> mermiler; // Mermi havuzu
        public Transform atesNoktasi; // Merminin çıkış noktası
        public float atesHizi = 0.5f; // Ateş hızı (cooldown)
        public AudioClip atesSesi; // Ateş sesi
        public Animator anim; // Ateş animasyonu
    }

    public List<Silah> silahlar = new List<Silah>();

    private Silah aktifSilah;
    private Dictionary<SilahTuru, float> sonAtesZamani = new Dictionary<SilahTuru, float>();
    private AudioSource sesKaynagi;
    public Button[] silahSecButonlar; // Silah seçim butonlarının array'i

    void Start()
    {
        sesKaynagi = GetComponent<AudioSource>();

        // Her silah için butonları ve işlemleri ayarla
        for (int i = 0; i < silahlar.Count; i++)
        {
            Silah silah = silahlar[i];

            // Ateş zamanlarını sıfırla
            if (!sonAtesZamani.ContainsKey(silah.silahTuru))
                sonAtesZamani[silah.silahTuru] = -silah.atesHizi;

            // Silahları başlangıçta devre dışı bırak
            silah.silahObjesi.SetActive(false);
            silah.atesButonu.gameObject.SetActive(false);

            // Tüm mermileri başlangıçta pasif yap
            foreach (GameObject mermi in silah.mermiler)
            {
                mermi.SetActive(false);
            }

            // Butonlara tıklandığında doğru silahı aktif et
            silahSecButonlar[i].onClick.AddListener(() => SilahSec(silah));
        }

        // İlk silahı seç
        if (silahlar.Count > 0)
        {
            SilahSec(silahlar[0]);
        }
    }

    // Silah seçme fonksiyonu
    void SilahSec(Silah yeniSilah)
    {
        if (aktifSilah != null)
        {
            aktifSilah.silahObjesi.SetActive(false);
            aktifSilah.atesButonu.gameObject.SetActive(false);
        }

        aktifSilah = yeniSilah;
        aktifSilah.silahObjesi.SetActive(true);
        aktifSilah.atesButonu.gameObject.SetActive(true);

        // Aktif silahın ateş butonuna ateş etme işlevini ekle
        aktifSilah.atesButonu.onClick.RemoveAllListeners();  // Önceden varsa önceki dinleyicileri temizle
        aktifSilah.atesButonu.onClick.AddListener(() => AtesEt());

        Debug.Log($"{aktifSilah.silahTuru} seçildi.");
    }

    // Ateş etme fonksiyonu
    void AtesEt()
    {
        if (aktifSilah != null)
        {
            float suankiZaman = Time.time;

            if (suankiZaman - sonAtesZamani[aktifSilah.silahTuru] >= aktifSilah.atesHizi)
            {
                // Kullanılmayan bir mermiyi aktif et
                GameObject mermi = aktifSilah.mermiler.Find(m => !m.activeInHierarchy);

                if (mermi != null)
                {
                    mermi.transform.position = aktifSilah.atesNoktasi.position;
                    mermi.transform.rotation = aktifSilah.atesNoktasi.rotation;
                    mermi.SetActive(true);

                    // Mermiyi hareket ettiren script'in resetlenmesi gerekiyorsa burada yapılabilir
                    var bulletScript = mermi.GetComponent<Bullet>();
                    if (bulletScript != null)
                    {
                        //bulletScript.ResetBullet(); // Eğer böyle bir metod varsa kullanılabilir.
                    }
                }
                else
                {
                    Debug.LogWarning("Mermi havuzunda kullanılabilir mermi kalmadı!");
                }

                // Ateş sesi çal
                if (aktifSilah.atesSesi != null)
                    sesKaynagi.PlayOneShot(aktifSilah.atesSesi);

                // Ateş animasyonu tetikle
                if (aktifSilah.anim != null)
                    aktifSilah.anim.SetTrigger("Ates");

                // Ateş zamanını güncelle
                sonAtesZamani[aktifSilah.silahTuru] = suankiZaman;

                Debug.Log($"{aktifSilah.silahTuru} ateş etti!");
            }
            else
            {
                Debug.Log("Ateş için bekleniyor...");
            }
        }
    }
}
