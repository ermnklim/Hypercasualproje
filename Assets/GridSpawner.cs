using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public GameObject cubeParent;
    public GameObject plane;
    public float cubeSize = 1.0f;

    void Start()
    {
        IzgaraOlustur();
    }

    void IzgaraOlustur()
    {
        MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
        float planeGenislik = planeRenderer.bounds.size.x;
        float planeYukseklik = planeRenderer.bounds.size.z;

        int izgaraGenislik = Mathf.FloorToInt(planeGenislik / cubeSize);
        int izgaraYukseklik = Mathf.FloorToInt(planeYukseklik / cubeSize);

        Transform[] tumKupler = cubeParent.GetComponentsInChildren<Transform>(true);
        int mevcutKupIndex = 1;

        for (int x = 0; x < izgaraGenislik && mevcutKupIndex < tumKupler.Length; x++)
        {
            for (int z = 0; z < izgaraYukseklik && mevcutKupIndex < tumKupler.Length; z++)
            {
                Vector3 kupPozisyon = new Vector3(
                    x * cubeSize - planeGenislik / 2 + cubeSize / 2,
                    0,
                    z * cubeSize - planeYukseklik / 2 + cubeSize / 2
                );

                tumKupler[mevcutKupIndex].gameObject.SetActive(true);
                tumKupler[mevcutKupIndex].position = kupPozisyon;
                mevcutKupIndex++;
            }
        }
    }
}