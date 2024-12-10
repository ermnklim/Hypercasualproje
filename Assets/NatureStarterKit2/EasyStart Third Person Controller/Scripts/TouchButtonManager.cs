using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TouchButtonManager : MonoBehaviour
{
    public GraphicRaycaster raycaster; // UI'deki tıklamaları algılamak için
    public EventSystem eventSystem;
    public RectTransform panelRect; // Butonların içinde kalması gereken panel
    public GridLayoutGroup gridLayout; // Sadece bu Grid Layout Group içindeki butonlar kontrol edilecek

    public float closestDistanceThreshold = 200f; // En yakın buton mesafesi

    private Button selectedButton; // Dokunulan butonu tutmak için
    private Vector2 previousTouchPosition;
    private Vector2 initialPosition; // Başlangıç pozisyonu
    private int originalIndex; // Sürüklenen butonun ilk sırası
    private Button swappedButton; // Yer değiştiren diğer buton

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Dokunulan UI elemanını tespit et
                    DetectTouchedButton(touch.position);
                    break;

                case TouchPhase.Moved:
                    // Eğer bir buton seçiliyse, hareket ettir
                    if (selectedButton != null && selectedButton.gameObject.activeSelf) // Buton aktifse
                    {
                        Vector2 touchDelta = touch.position - previousTouchPosition;
                        MoveButton(selectedButton, touchDelta);
                    }
                    break;

                case TouchPhase.Ended:
                    // Parmağı kaldırınca işlem yap
                    if (selectedButton != null)
                    {
                        CheckButtonBoundsAndReorder(selectedButton, touch.position);
                        selectedButton = null; // Seçili butonu null yap
                    }
                    break;

                case TouchPhase.Canceled:
                    // Parmağı kaldırınca butonu bırak
                    selectedButton = null; // Seçili butonu null yap
                    break;
            }

            previousTouchPosition = touch.position; // Dokunma pozisyonunu güncelle
        }
    }

    void DetectTouchedButton(Vector2 touchPosition)
    {
        // Dokunulan UI elemanını bulmak için bir PointerEventData oluştur
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touchPosition;

        // UI elemanlarını algıla
        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        // İlk buton olan sonucu seç
        foreach (var result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null && button.transform.parent == gridLayout.transform) // Yalnızca Grid Layout'taki butonları al
            {
                selectedButton = button;
                initialPosition = button.GetComponent<RectTransform>().anchoredPosition; // Başlangıç pozisyonu
                originalIndex = button.transform.GetSiblingIndex(); // Butonun sırasını kaydet
                break;
            }
        }
    }

    void MoveButton(Button button, Vector2 touchDelta)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Hareket ettir
            rectTransform.anchoredPosition += new Vector2(touchDelta.x, touchDelta.y);
        }
    }

    void CheckButtonBoundsAndReorder(Button button, Vector2 touchPosition)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();

        // Eğer buton panelin sınırları dışındaysa eski pozisyona geri dön
        if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, buttonRect.position))
        {
            // Panel dışına çıktıysa eski pozisyona geri döndür
            button.GetComponent<RectTransform>().anchoredPosition = initialPosition;
            button.transform.SetSiblingIndex(originalIndex); // İlk sırasına geri döndür
            Debug.Log($"{button.name} panelin dışına çıktı ve eski pozisyonuna geri döndü.");
            return;
        }

        // Hedef butonu belirle
        Button targetButton = GetClosestButton(touchPosition, button);

        if (targetButton != null && targetButton != swappedButton) // Yer değiştiren butonlardan farklı biri
        {
            // Hedef butonun sırasını al ve sıraları değiştir
            int targetIndex = targetButton.transform.GetSiblingIndex();
            int selectedButtonIndex = selectedButton.transform.GetSiblingIndex();

            // Sıraları değiştir
            selectedButton.transform.SetSiblingIndex(targetIndex);
            targetButton.transform.SetSiblingIndex(selectedButtonIndex);

            // Yer değiştiren butonları kaydet
            swappedButton = targetButton;
            Debug.Log($"Buton sırası değiştirildi: {selectedButton.name} <-> {targetButton.name}");
        }
        else
        {
            // Eğer başka bir butona bırakılmadıysa eski pozisyonuna geri döndür
            selectedButton.GetComponent<RectTransform>().anchoredPosition = initialPosition;
            selectedButton.transform.SetSiblingIndex(originalIndex);
            Debug.Log($"{selectedButton.name} başka bir butona değmedi ve eski pozisyonuna geri döndü.");
        }

        swappedButton = null; // Sırası değişen butonu sıfırla
    }

    Button GetClosestButton(Vector2 touchPosition, Button ignoreButton)
    {
        Button closestButton = null;
        float closestDistance = float.MaxValue;

        // Grid Layout içindeki tüm butonları kontrol et
        foreach (Transform child in gridLayout.transform)
        {
            if (child == ignoreButton.transform) continue;

            RectTransform childRect = child.GetComponent<RectTransform>();
            Vector2 childPosition = childRect.position;
            float distance = Vector2.Distance(touchPosition, childPosition);

            // En yakın butonu belirle
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestButton = child.GetComponent<Button>();
            }
        }

        // Mesafe threshold'a göre en yakın buton
        return closestDistance < closestDistanceThreshold ? closestButton : null;
    }
}
