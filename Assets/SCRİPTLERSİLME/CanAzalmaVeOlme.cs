using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjeType
{
    Agac,
    Tas,
    Dusman,
    Meyve,
    Diger
}

public class CanAzalmaVeOlme : MonoBehaviour
{
    [SerializeField] private float can = 100f;
    [SerializeField] private float devrilmeHizi = 95f;
    [SerializeField] private bool xEksenindeDevril = true;
    [SerializeField] private List<GameObject> aktiflesecekObjeler;
    [SerializeField] private int minAktifObjeSayisi = 1;
    [SerializeField] private int maxAktifObjeSayisi = 3;
    [SerializeField] private ObjeType objeType = ObjeType.Agac;
    
    private bool oluyor = false;
    private bool objelerAktiflesti = false;

    private void Update()
    {
        if (oluyor)
        {
            if (objeType == ObjeType.Agac)
            {
                Vector3 mevcutRotasyon = transform.localRotation.eulerAngles;
                float hedefAci = xEksenindeDevril ? 90f : mevcutRotasyon.x;
                float hedefAciZ = xEksenindeDevril ? mevcutRotasyon.z : 90f;

                transform.localRotation = Quaternion.Euler(
                    Mathf.LerpAngle(mevcutRotasyon.x, hedefAci, devrilmeHizi * Time.deltaTime / 40f),
                    mevcutRotasyon.y,
                    Mathf.LerpAngle(mevcutRotasyon.z, hedefAciZ, devrilmeHizi * Time.deltaTime / 40f)
                );

                if (!objelerAktiflesti &&
                    (xEksenindeDevril && Mathf.Approximately(transform.localRotation.eulerAngles.x, 90f) ||
                    !xEksenindeDevril && Mathf.Approximately(transform.localRotation.eulerAngles.z, 90f)))
                {
                    RastgeleObjeleriAktiflestir();
                }
            }
        }
    }

    public void HasarAl(float hasar)
    {
        if (oluyor) return;

        can -= hasar;
        if (can <= 0)
        {
            if (objeType == ObjeType.Agac)
            {
                OlmeyeBasla();
            }
            else if (objeType == ObjeType.Tas)
            {
                TasIcinDeaktifEt();
            }
        }
    }

    private void OlmeyeBasla()
    {
        oluyor = true;
        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }
    }

    private void TasIcinDeaktifEt()
    {
        // Direkt olarak rastgele objeleri aktif et
        RastgeleObjeleriAktiflestir();

        // Nesneyi deaktif et
        gameObject.SetActive(false);
    }

    private void RastgeleObjeleriAktiflestir()
    {
        objelerAktiflesti = true;
        Vector3 baseKonum = transform.position;

        // Deaktif olan objeleri filtrele
        List<GameObject> deaktifObjeler = aktiflesecekObjeler.FindAll(obj => obj != null && !obj.activeSelf);

        // Aktifleşecek obje sayısını belirle
        int aktiflesecekObjeSayisi = Mathf.Min(
            Random.Range(minAktifObjeSayisi, maxAktifObjeSayisi + 1), 
            deaktifObjeler.Count
        );

        // Rastgele seçim için listeyi karıştır
        for (int i = deaktifObjeler.Count - 1; i > 0; i--)
        {
            int rastgeleIndex = Random.Range(0, i + 1);
            GameObject gecici = deaktifObjeler[i];
            deaktifObjeler[i] = deaktifObjeler[rastgeleIndex];
            deaktifObjeler[rastgeleIndex] = gecici;
        }

        // Belirlenen sayıda objeyi aktif hale getir
        for (int i = 0; i < aktiflesecekObjeSayisi; i++)
        {
            if (deaktifObjeler[i] != null)
            {
                // Rastgele bir pozisyon oluştur
                Vector3 rastgelePosisyon = baseKonum + new Vector3(
                    Random.Range(-2f, 2f),   // X ekseninde küçük bir rastgelelik
                    3f,                      // Sabit olarak 3 metre yükseklik
                    Random.Range(-2f, 2f)    // Z ekseninde küçük bir rastgelelik
                );

                deaktifObjeler[i].transform.position = rastgelePosisyon;
                deaktifObjeler[i].SetActive(true);
            }
        }

        if (objeType == ObjeType.Agac)
        {
            StartCoroutine(GecikmeliDeaktifEt());
        }
    }

    private IEnumerator GecikmeliDeaktifEt()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
