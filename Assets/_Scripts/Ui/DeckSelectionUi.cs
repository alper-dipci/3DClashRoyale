using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeckSelectionUi : MonoBehaviour
{
    public List<Vector2> cardsPosList;
    public RectTransform CardsParent;
    private void Start()
    {
        foreach (RectTransform t in CardsParent)
        {
            cardsPosList.Add(t.anchoredPosition);
        }
    }
    public void RefreshCards()
    {
        for (int i=0; i<CardsParent.childCount;i++)
        {
            CardsParent.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(cardsPosList[i],.2f);
        }
    }
}
