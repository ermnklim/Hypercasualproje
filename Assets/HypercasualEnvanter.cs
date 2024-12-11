using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HypercasualEnvanter : MonoBehaviour
{
    public enum EnvanterTurleri
    {
        Altin,
        Tas,
        Agac,
        Et,
        Meyve
    }

    [System.Serializable]
    public class EnvanterObjesi
    {
        public EnvanterTurleri envanterTuru;
        public int miktar;
        public Image envanterResmi;
        public TextMeshProUGUI envanterMiktarText;
    }

    [SerializeField] private List<EnvanterObjesi> envanterObjeleri = new List<EnvanterObjesi>();

    void Start()
    {
        EnvanterGorunumuGuncelle();
    }

    void EnvanterGorunumuGuncelle()
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.miktar > 0)
            {
                obje.envanterResmi.gameObject.SetActive(true);
            }
            else
            {
                obje.envanterResmi.gameObject.SetActive(false);
            }

            if (obje.envanterMiktarText != null)
            {
                obje.envanterMiktarText.text = obje.miktar.ToString();
            }
        }
    }

    public void MiktarArttir(EnvanterTurleri tur, int miktar)
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.envanterTuru == tur)
            {
                obje.miktar += miktar;
                break;
            }
        }
        EnvanterGorunumuGuncelle();
    }

    public void MiktarAzalt(EnvanterTurleri tur, int miktar)
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.envanterTuru == tur)
            {
                obje.miktar = Mathf.Max(0, obje.miktar - miktar);
                break;
            }
        }
        EnvanterGorunumuGuncelle();
    }

    public void MiktarGuncelle(EnvanterTurleri tur, int yeniMiktar)
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.envanterTuru == tur)
            {
                obje.miktar = yeniMiktar;
                break;
            }
        }
        EnvanterGorunumuGuncelle();
    }

    public int KaynakMiktariniAl(EnvanterTurleri tur)
    {
        foreach (EnvanterObjesi obje in envanterObjeleri)
        {
            if (obje.envanterTuru == tur)
            {
                return obje.miktar;
            }
        }
        return 0;
    }
}
