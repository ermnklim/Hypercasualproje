using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mermi : MonoBehaviour
{
    public string hedefIsmi; // Çarpışma kontrolü için hedef nesnenin adı

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == hedefIsmi)
        {
            Debug.Log("Hedef vuruldu: " + other.name);
            
            // Mermiyi pasifleştir
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Çarpılan nesnenin ismi ile kontrol edilen isim eşleşiyorsa
        if (collision.collider.name == hedefIsmi)
        {
            Debug.Log("Hedef vuruldu: " + collision.collider.name);
            
            // Mermiyi pasifleştir
            gameObject.SetActive(false);
        }
    }
}