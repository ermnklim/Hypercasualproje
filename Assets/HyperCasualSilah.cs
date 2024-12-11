using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HyperCasualEkipman : MonoBehaviour
{
    public GameObject aciklamaPanel;
    public TextMeshProUGUI aciklamaText;
    public List<Ekipman> ekipmanlar = new List<Ekipman>();
    public enum EkipmanTuru
    {
        TekEl,
        CiftEl
    }

    [System.Serializable]
    public class Ekipman
    {
        public EkipmanTuru tur;
        public string isim;
        public string aciklama;
        public float vurmaHiziEtkisi;
        public float saldiriMesafesiEtkisi;
        public float hasarEtkisi;
        public float canEtkisi;
        public Button equipButton;
        public Button ozellikButton;
        public GameObject model;
    }

    private void Awake()
    {
        foreach (var ekipman in ekipmanlar)
        {
            ekipman.ozellikButton.onClick.AddListener(() => EkipmanOzellikGoster(ekipman));
            ekipman.equipButton.onClick.AddListener(() => EkipmanSec(ekipman));
        }
    }

    private void EkipmanOzellikGoster(Ekipman ekipman)
    {
        aciklamaPanel.SetActive(true);
        string bilgiler = $"İsim: {ekipman.isim}\n" +
                          $"Tür: {ekipman.tur}\n" +
                          $"Açıklama: {ekipman.aciklama}\n\n" +
                          $"Özellikler:\n" +
                          $"Vurma Hızı Etkisi: +{ekipman.vurmaHiziEtkisi}\n" +
                          $"Saldırı Mesafesi Etkisi: +{ekipman.saldiriMesafesiEtkisi}\n" +
                          $"Hasar Etkisi: +{ekipman.hasarEtkisi}\n" +
                          $"Can Etkisi: +{ekipman.canEtkisi}";

        aciklamaText.text = bilgiler;
    }
    private void EkipmanSec(Ekipman secilenEkipman)
    {
        foreach (var ekipman in ekipmanlar)
        {
            ekipman.model.SetActive(false);
        }
        secilenEkipman.model.SetActive(true);
        Debug.Log($"{secilenEkipman.isim} seçildi ve aktif hale getirildi.");
    }

    public void AciklamaPaneliKapat()
    {
        aciklamaPanel.SetActive(false);
        aciklamaText.text = null;
    }
}
