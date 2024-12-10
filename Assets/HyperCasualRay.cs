using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HyperCasualRay : MonoBehaviour
{
    public Animator animator;
    public float rayDistance = 10f;
    public Transform raypoz;
    public HypercasualEnvanter envanter;
    private GameObject lastHitObject;
    private string currentAnimation;
    public TextMeshProUGUI text;
    public List<GameObject> aktifsilah;

    public void SetAktifSilah(int index)
    {
        if (index >= 0 && index < aktifsilah.Count)
        {
            foreach (GameObject silah in aktifsilah)
            {
                silah.SetActive(false);
            }

            aktifsilah[index].SetActive(true);
        }
        else
        {
            foreach (GameObject silah in aktifsilah)
            {
                silah.SetActive(false);
            }
        }
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(raypoz.transform.position, transform.forward);
        Debug.DrawRay(raypoz.transform.position, transform.forward * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != lastHitObject)
            {
                lastHitObject = hitObject;

                if (!string.IsNullOrEmpty(currentAnimation))
                {
                    animator.SetBool(currentAnimation, false);
                }

                Can canComponent = hitObject.GetComponent<Can>();

                if (hitObject.CompareTag("Agac") && canComponent != null)
                {
                    PlayHitAnimation("Kes", HypercasualEnvanter.EnvanterTurleri.Agac, canComponent, 10f);
                    SetAktifSilah(0);
                }
                else if (hitObject.CompareTag("Tas") && canComponent != null)
                {
                    PlayHitAnimation("Kir", HypercasualEnvanter.EnvanterTurleri.Tas, canComponent, 15f);
                    SetAktifSilah(1);
                }
                else if (hitObject.CompareTag("Altin") && canComponent != null)
                {
                    PlayHitAnimation("Kir", HypercasualEnvanter.EnvanterTurleri.Altin, canComponent, 25f);
                    SetAktifSilah(1);
                }
                else if (hitObject.CompareTag("Hayvan") && canComponent != null)
                {
                    PlayHitAnimation("Kes", HypercasualEnvanter.EnvanterTurleri.Et, canComponent, 20f);
                    SetAktifSilah(2);
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(currentAnimation))
            {
                animator.SetBool(currentAnimation, false);
                currentAnimation = null;
                text.text = null;
                SetAktifSilah(-1);
            }

            lastHitObject = null;
        }
    }

    private void PlayHitAnimation(string animationBool, HypercasualEnvanter.EnvanterTurleri envanterTuru, Can canComponent, float canAzalmaMiktari)
    {
        animator.SetBool(animationBool, true);
        currentAnimation = animationBool;
        StartCoroutine(HitCoroutine(canComponent, canAzalmaMiktari, envanterTuru));
    }

    private IEnumerator HitCoroutine(Can canComponent, float canAzalmaMiktari, HypercasualEnvanter.EnvanterTurleri envanterTuru)
    {
        while (lastHitObject != null && animator.GetBool(currentAnimation))
        {
            if (canComponent != null)
            {
                canComponent.DecreaseCan(canAzalmaMiktari);

                if (canComponent.mycan <= 0)
                {
                    animator.SetBool(currentAnimation, false);
                    break;
                }
            }

            if (envanter != null)
            {
                envanter.MiktarArttir(envanterTuru, 1);

                string kaynakIsmi = envanterTuru.ToString();
                int miktar = envanter.KaynakMiktariniAl(envanterTuru);

                text.text = $"{kaynakIsmi + "+"}: {miktar}";
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
