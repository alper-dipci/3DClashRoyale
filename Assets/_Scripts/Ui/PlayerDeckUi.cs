using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PlayerDeckUi : MonoBehaviour
{
    [SerializeField] List<Image> images;
    [SerializeField] private float cardAnimSpeed;

    private Vector2 nextCardRectPos;
    private Vector2 nextCardRectScale;
    private void Start()
    {
        nextCardRectPos = images[4].rectTransform.anchoredPosition;
        nextCardRectScale = images[4].rectTransform.localScale;
    }
    public IEnumerator updateList(List<ScriptableExampleHero> heroDeck, int heroIndexinList)
    {
        images[4].rectTransform.DOAnchorPos(images[heroIndexinList].rectTransform.anchoredPosition, cardAnimSpeed, false).SetEase(Ease.OutBack);
        images[4].rectTransform.DOScale(images[heroIndexinList].rectTransform.localScale, cardAnimSpeed).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(cardAnimSpeed);
        images[4].rectTransform.anchoredPosition= nextCardRectPos;
        images[4].rectTransform.localScale = nextCardRectScale;
        updateList(heroDeck);
        //images[4].sprite = heroDeck[4].HeroSprite;
    }
    public void updateList(List<ScriptableExampleHero> heroDeck)
    {
        for(int i = 0; i < images.Count; i++)
        {
            images[i].sprite = heroDeck[i].HeroSprite;
        }
    }
    //private Vector2 getDistanceToImage(int heroIndex) { (Vector2)images[heroIndex].rectTransform.anchoredPosition - (Vector2)images[4].rectTransform.position; }
}
