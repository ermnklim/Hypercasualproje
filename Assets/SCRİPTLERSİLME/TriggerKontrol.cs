using UnityEngine;

public class TriggerKontrol : MonoBehaviour
{
    public bool dokunmaEnabled = false;

    private void OnTriggerEnter(Collider other)
    {
        // Eğer başka bir nesneye değiyorsa, dokunmaEnabled'ı true yap
        dokunmaEnabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Eğer nesne temas etmiyorsa, dokunmaEnabled'ı false yap
        dokunmaEnabled = false;
    }
}