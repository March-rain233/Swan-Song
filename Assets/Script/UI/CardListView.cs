using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameToolKit;

public class CardListView : MonoBehaviour
{
    public LayoutGroup LayoutGroup;
    public virtual void ShowCardList(IEnumerable<Card> cards)
    {
        for(int i = LayoutGroup.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(LayoutGroup.transform.GetChild(i).gameObject);
        }
        var model = UISetting.Instance.PrefabsDic["CardView"];
        foreach (Card card in cards)
        {
            var cardView = Instantiate(model, LayoutGroup.transform).GetComponent<CardView>();
            cardView.Card = card;
            cardView.Refresh();
            cardView.MouseEntered += () =>
            {
                cardView.transform.DOScale(1.1f, BattleAnimator.ShortAnimationDuration);
            };
            cardView.MouseExited += () =>
            {
                cardView.transform.DOScale(1, BattleAnimator.ShortAnimationDuration);
            };
        }
    }
}