

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour,IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image image;
    private Vector2 firstPos;

    public ItemSlot CardSlot;

    public RectTransform CardsParent;
    public DeckSelectionUi deckSelectionUi;
    public ScriptableExampleHero heroSo;

    private void Awake() {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image.sprite = heroSo.HeroSprite;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (CardSlot)
        {
            CardSlot.HeroSO = null;
            CardSlot = null;
        }
        
        firstPos = rectTransform.anchoredPosition;
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (!CardSlot)
        {
            rectTransform.anchoredPosition = firstPos;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(1) && CardSlot)
        {
            CardSlot.HeroSO = null;
            CardSlot = null;
            rectTransform.SetParent(CardsParent);
            deckSelectionUi.RefreshCards();
        }
    }
}
