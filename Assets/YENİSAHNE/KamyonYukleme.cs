using System;
using TMPro;
using UnityEngine;
public class KamyonYukleme : MonoBehaviour
{
   public float kalansure;
   public int kalankasa;
   public TextMeshProUGUI suretext, kasatext;
   public AudioSource ses;
   public GameObject oldunpanel,yenisahne;
   private void FixedUpdate()
   {
      kalansure -= Time.fixedDeltaTime;
      if (kalansure==0)
      {
        
           // ses.Play();
            //oldunpanel.SetActive(true);
            kalansure = 0;
         
      }

      if (kalankasa==0)
      {
         kalankasa = 0;
         //yenisahne.SetActive(true);
      }
   }

   private void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.CompareTag("Kasa"))
      {
         kalankasa--;
      }
   }
}
