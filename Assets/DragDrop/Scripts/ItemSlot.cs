/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler {

    public int CardIndex;
    public ScriptableExampleHero HeroSO;
    public ScriptableHeroDeck heroDeck;
    public DeckSelectionUi deckSelectionUi;
    public void OnDrop(PointerEventData eventData) {
        if (HeroSO != null) return;
        if (eventData.pointerDrag != null) {

            //set card position to slot position
            RectTransform CardRectTransform = eventData.pointerDrag.GetComponent<RectTransform>();           
            CardRectTransform.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;            
            DragDrop dragDrop = eventData.pointerDrag.GetComponent<DragDrop>();

            //set this slots heroSo from card
            HeroSO = dragDrop.heroSo;

            //if card was in another slot clear the other slot
            if(dragDrop.CardSlot!=null)
                dragDrop.CardSlot.HeroSO = null;

            //set cards slot as this
            dragDrop.CardSlot=this;

            //set the card to the actual deckSO
            heroDeck.Heroes[CardIndex] = HeroSO;

            //set card parent to another parent so deck can refresh normally
            CardRectTransform.SetParent(deckSelectionUi.transform);
            deckSelectionUi.RefreshCards();
        }
    }

}
