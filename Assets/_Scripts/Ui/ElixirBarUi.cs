using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class ElixirBarUi : MonoBehaviour
{
    [SerializeField] Image fillerImage;
    [SerializeField] Image elixirImage;
    [SerializeField] TextMeshProUGUI ElixirText;
    private float barElixirTarget;
    [SerializeField] float barElixirAnimDuration;

    public void UpdateBarFiller(float barFillerAmount)
    {
        fillerImage.fillAmount = elixirImage.fillAmount+barFillerAmount;
    }
    public void UpdateBarElixir(int barElixirAmount)
    {
        ElixirText.text = barElixirAmount.ToString();
        elixirImage.DOFillAmount((float)barElixirAmount/10, barElixirAnimDuration).SetEase(Ease.OutBack);
    }

}
