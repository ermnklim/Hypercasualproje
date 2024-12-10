using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonControllerMobile : MonoBehaviour
{
    [Header("UI Elementleri")]
    public FixedJoystick joystick;
    public Button ziplamaButonu;
    public Button egilmeButonu;
    public Button kosmaButonu;
    public Button karakterDegistirmeButonu;

    [Header("Karakter Ayarları")]
    public float hiz = 5f;
    public float kosmaHizArtisi = 3.5f;
    public float ziplamaGucu = 18f;
    public float ziplamaSuresi = 0.85f;
    public float yercekimi = 9.8f;

    [Header("Karakter GameObject'leri")]
    public GameObject[] karakterler;
    private int aktifKarakterIndex = 0;

    private float ziplamaGecenSure = 0;
    private bool ziplamaAktif = false;
    private bool kosmaAktif = false;
    private bool egilmeAktif = false;

    private Animator[] animatorler;
    private CharacterController karakterKontrol;

    private void Start()
    {
        karakterKontrol = GetComponent<CharacterController>();
        
        // Tüm karakterlerin animatorlerini al
        animatorler = new Animator[karakterler.Length];
        for (int i = 0; i < karakterler.Length; i++)
        {
            animatorler[i] = karakterler[i].GetComponent<Animator>();
            if (animatorler[i] == null)
            {
                Debug.LogWarning($"Animator bileşeni bulunamadı: Karakter {i}");
            }
            
            // İlk karakter hariç diğerlerini kapat
            if (i != aktifKarakterIndex)
            {
                karakterler[i].SetActive(false);
            }
        }

        // Buton dinleyicileri
   
        karakterDegistirmeButonu.onClick.AddListener(SonrakiKaraktereGec);
    }

    private void SonrakiKaraktereGec()
    {
        // Aktif karakteri kapat
        karakterler[aktifKarakterIndex].SetActive(false);
        
        // Sonraki karaktere geç
        aktifKarakterIndex = (aktifKarakterIndex + 1) % karakterler.Length;
        
        // Yeni karakteri aç
        karakterler[aktifKarakterIndex].SetActive(true);
    }

    private void Update()
    {
        float yatayGiris = joystick.Horizontal;
        float dikeyGiris = joystick.Vertical;

        if (kosmaAktif && Mathf.Abs(yatayGiris) < 0.1f && Mathf.Abs(dikeyGiris) < 0.1f)
        {
            kosmaAktif = false;
        }

        if (karakterKontrol.isGrounded && animatorler[aktifKarakterIndex] != null)
        {
            animatorler[aktifKarakterIndex].SetBool("run", karakterKontrol.velocity.magnitude > 0.9f);
            animatorler[aktifKarakterIndex].SetBool("sprint", kosmaAktif);
            animatorler[aktifKarakterIndex].SetBool("crouch", egilmeAktif);
        }

        if (animatorler[aktifKarakterIndex] != null)
            animatorler[aktifKarakterIndex].SetBool("air", !karakterKontrol.isGrounded);

        Hareket(yatayGiris, dikeyGiris);
    }

    private void KosmaAktifEt()
    {
        kosmaAktif = true;
    }

    private void Hareket(float yatayGiris, float dikeyGiris)
    {
        float hizArtisi = kosmaAktif ? kosmaHizArtisi : 0;
        if (egilmeAktif) hizArtisi = -(hiz * 0.5f);

        float yonX = yatayGiris * (hiz + hizArtisi) * Time.deltaTime;
        float yonZ = dikeyGiris * (hiz + hizArtisi) * Time.deltaTime;
        float yonY = 0;

        if (ziplamaAktif)
        {
            yonY = Mathf.SmoothStep(ziplamaGucu, ziplamaGucu * 0.3f, ziplamaGecenSure / ziplamaSuresi) * Time.deltaTime;
            ziplamaGecenSure += Time.deltaTime;
            if (ziplamaGecenSure >= ziplamaSuresi)
            {
                ziplamaAktif = false;
                ziplamaGecenSure = 0;
            }
        }

        yonY -= yercekimi * Time.deltaTime;

        Vector3 ileri = Camera.main.transform.forward;
        Vector3 sag = Camera.main.transform.right;

        ileri.y = 0;
        sag.y = 0;
        ileri.Normalize();
        sag.Normalize();

        Vector3 hareketYonu = ileri * yonZ + sag * yonX;
        if (hareketYonu.magnitude > 0)
        {
            Quaternion hedefRotasyon = Quaternion.LookRotation(hareketYonu);
            transform.rotation = Quaternion.Slerp(transform.rotation, hedefRotasyon, 0.15f);
        }

        Vector3 hareket = hareketYonu + Vector3.up * yonY;
        karakterKontrol.Move(hareket);
    }

    private void Zipla()
    {
        if (karakterKontrol.isGrounded)
        {
            ziplamaAktif = true;
        }
    }

    private void EgilmeDegistir()
    {
        egilmeAktif = !egilmeAktif;
    }
}