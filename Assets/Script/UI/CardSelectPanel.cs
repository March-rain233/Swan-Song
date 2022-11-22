using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using UnityEngine.UI;
using System;
using TMPro;

public class CardSelectPanel : PanelBase
{
    public CardListView CardListView;
    public TextMeshProUGUI TxtTitle;

    public event Action<Card> OnCardSelected;
    protected override void OnInit()
    {
        base.OnInit();
        CardListView.InittingCardView += CardListView_InittingCardView;
    }

    private void CardListView_InittingCardView(CardView obj)
    {
        obj.Clicked += () =>
        {
            OnCardSelected?.Invoke(obj.Card);
        };
    }

    public void SetCards(IEnumerable<Card> cards)
    {
        CardListView.ShowCardList(cards);
    }
}
