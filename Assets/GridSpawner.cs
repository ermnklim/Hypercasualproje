using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public GameObject cubeParent; // Küplerin parent objesi
    public GameObject plane; // Plane objesi
    public float cubeSize = 1.0f; // Küp boyutu

    void Start()
    {
        IzgaraOlustur();
    }

    void IzgaraOlustur()
    {
        // Plane'nin boyutlarını alıyoruz
        MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
        float planeGenislik = planeRenderer.bounds.size.x;
        float planeYukseklik = planeRenderer.bounds.size.z;

        // Izgara boyutlarını hesaplıyoruz
        int izgaraGenislik = Mathf.FloorToInt(planeGenislik / cubeSize);
        int izgaraYukseklik = Mathf.FloorToInt(planeYukseklik / cubeSize);

        // Mevcut tüm child objeleri alıyoruz
        Transform[] tumKupler = cubeParent.GetComponentsInChildren<Transform>(true);
        int mevcutKupIndex = 1; // 0. indeks parent objenin kendisi olduğu için 1'den başlıyoruz

        // Küpleri yerleştiriyoruz
        for (int x = 0; x < izgaraGenislik && mevcutKupIndex < tumKupler.Length; x++)
        {
            for (int z = 0; z < izgaraYukseklik && mevcutKupIndex < tumKupler.Length; z++)
            {
                // Küpün pozisyonunu hesaplıyoruz
                Vector3 kupPozisyon = new Vector3(
                    x * cubeSize - planeGenislik / 2 + cubeSize / 2,
                    0,
                    z * cubeSize - planeYukseklik / 2 + cubeSize / 2
                );

                // Mevcut küpü aktifleştirip pozisyonunu ayarlıyoruz
                tumKupler[mevcutKupIndex].gameObject.SetActive(true);
                tumKupler[mevcutKupIndex].position = kupPozisyon;
                mevcutKupIndex++;
            }
        }
    }
}