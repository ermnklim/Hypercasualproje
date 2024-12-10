using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class bayrak : MonoBehaviour
{
    public Transform dikpozison; // Bayrak yerleştirileceği pozisyon
    public Button dikbuton; // "Yerleştir" butonu
    public GameObject askerler, hareketpanel; // Askerler ve hareket paneli
    public Camera mainCamera; // Ana kamera

    private Vector3 originalCameraPosition; // Kameranın orijinal pozisyonu
    private Quaternion originalCameraRotation; // Kameranın orijinal rotasyonu
    public GameObject Konusma_Panel,player; // Konuşma paneli
    public Image fadeImage; // Ekran kararma efekti için Image
    public string sceneName;

    private void Start()
    {
        // Buton dinleyicisini ekle
        dikbuton.onClick.AddListener(Yerlestir);

        // Kameranın orijinal pozisyonunu ve rotasyonunu kaydet
        originalCameraPosition = mainCamera.transform.localPosition;
        originalCameraRotation = mainCamera.transform.localRotation;

        // Fade image başlangıçta tamamen şeffaf olmalı
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
    }

    public void Yerlestir()
    {
        // Bayrak yerleştir
        gameObject.transform.SetParent(null);
        gameObject.transform.position = dikpozison.position;
        gameObject.transform.rotation = Quaternion.identity;
        askerler.transform.position = player.transform.position;

        // Askerleri aktif et ve hareket panelini kapat
        askerler.SetActive(true);
        hareketpanel.SetActive(false);

        // "Yerleştir" butonunu kapat
        dikbuton.gameObject.SetActive(false);

        // Kamerayı sıfırla ve döndürme işlemini başlat
        StartCoroutine(ResetRotateDeactivate());
    }

    private IEnumerator ResetRotateDeactivate()
    {
        // Kamerayı sıfır pozisyonuna ve rotasyonuna yavaşça taşı
        float duration = 1.5f; // Geçiş süresi
        Vector3 targetPosition = Vector3.zero; // Kameranın hedef pozisyonu
        Quaternion targetRotation = Quaternion.identity; // Kameranın hedef rotasyonu

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Kamerayı pozisyon ve rotasyon için interpolasyon yap
            mainCamera.transform.localPosition = Vector3.Lerp(originalCameraPosition, targetPosition, t);
            mainCamera.transform.localRotation = Quaternion.Slerp(originalCameraRotation, targetRotation, t);

            yield return null;
        }

        // Kamerayı 360 derece döndürme işlemi
        float rotationDuration = 2f; // Döndürme süresi
        float rotationElapsed = 0f;
        while (rotationElapsed < rotationDuration)
        {
            rotationElapsed += Time.deltaTime;
            float angle = Mathf.Lerp(0f, 360f, rotationElapsed / rotationDuration);
            mainCamera.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }

        // Kamerayı orijinal pozisyonuna ve rotasyonuna geri döndür
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            mainCamera.transform.localPosition = Vector3.Lerp(targetPosition, originalCameraPosition, t);
            mainCamera.transform.localRotation = Quaternion.Slerp(targetRotation, originalCameraRotation, t);
            Konusma_Panel.SetActive(true);
            yield return null;
        }

        // Fade out (ekranı karart)
        yield return StartCoroutine(FadeOut());

        // Konuşma panelini aç
       

        // Tüm işlemler tamamlandıktan sonra bu GameObject'i devre dışı bırak
        //gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        float fadeDuration = 5f; // 5 saniye boyunca kararma
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Fade image alfa değerini artır (kararma)
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0f, 1f, t); // 0'dan 1'e yavaşça geçiş
            fadeImage.color = color;

            yield return null;
        }
        Konusma_Panel.SetActive(false);
        SceneManager.LoadScene(sceneName);
    }

}
