using System;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Can : MonoBehaviour
{
    public float mycan, maxcan;
    public float minTimeToReactivate = 2f;
    public float maxTimeToReactivate = 5f;
    private bool isDeactivating = false;
    public bool small;

    public HypercasualEnvanter envanter; 
    public HypercasualEnvanter.EnvanterTurleri envanterTuru; 

    public TextMeshProUGUI envanterText; 

    void Start()
    {
        mycan = maxcan; 
        UpdateUI(); 
    }

    void Update()
    {
        if (mycan <= 0)
        {
            mycan = 0;
            if (!isDeactivating) 
            {
                isDeactivating = true;
                Invoke("ReactivateCan", Random.Range(minTimeToReactivate, maxTimeToReactivate));
                gameObject.SetActive(false); 
            }
        }

        UpdateUI(); 
    }

    private void ReactivateCan()
    {
        mycan = maxcan; 
        isDeactivating = false; 
        gameObject.SetActive(true);
    }

    public void IncreaseCan(float amount)
    {
        mycan += amount;
        if (mycan > maxcan) mycan = maxcan; 
        UpdateUI(); 
    }

    public void DecreaseCan(float amount)
    {
        mycan -= amount;
        if (mycan < 0) mycan = 0;
        UpdateUI();
    }

   
    private void UpdateUI()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (small)
            {
                if (envanter != null)
                {
                    envanter.MiktarArttir(envanterTuru, 1);
                    if (envanterText != null && envanter != null)
                    {
                        int envanterMiktari = envanter.KaynakMiktariniAl(envanterTuru); 
                        envanterText.text = $"{envanterTuru.ToString()}: {envanterMiktari}"; 
                    }
                    gameObject.SetActive(false);
                   Invoke("disabletext",1f);
                }
            }
        }
    }

    private void disabletext()
    {
        envanterText.text = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (small)
            {
                if (envanter != null)
                {
                    envanter.MiktarArttir(envanterTuru, 1);
                    if (envanterText != null && envanter != null)
                    {
                        int envanterMiktari = envanter.KaynakMiktariniAl(envanterTuru); 
                        envanterText.text = $"{envanterTuru.ToString()}: {envanterMiktari}";
                    }
                    gameObject.SetActive(false); 
                    Invoke("disabletext",.3f);
                }
            }
        }
    }
}
