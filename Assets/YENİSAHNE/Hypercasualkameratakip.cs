using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hypercasualkameratakip : MonoBehaviour
{
    public Transform target; // Takip edilecek nesne (karakter)
    public Vector3 offset; // Kameranın hedefe olan ofseti
    public float followSpeed = 5f; // Takip hızı

    private void LateUpdate()
    {
        if (target == null) return;

        // Hedef pozisyonu hesapla
        Vector3 targetPosition = target.position + offset;

        // Kamerayı hedef pozisyona doğru hareket ettir
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Kameranın hedefe bakmasını sağla (isteğe bağlı)
        transform.LookAt(target);
    }
}