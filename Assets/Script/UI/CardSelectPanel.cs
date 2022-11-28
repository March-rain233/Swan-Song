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
    public Button BtnBack;
    public event Action<Card, UnitData> CardSelected;
    public event Action Quitting;
    protected override void OnInit()
    {
        base.OnInit();
        CardListView.InittingCardView += CardListView_InittingCardView;
        BtnBack.onClick.AddListener(() =>
        {
            Quitting?.Invoke();
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(this);
        });
    }

    private void CardListView_InittingCardView(CardView obj)
    {
        obj.Clicked += () =>
        {
            CardSelected?.Invoke(obj.Card, obj.UnitData);
        };
    }

    public void SetCards(IEnumerable<(Card, UnitData)> cards)
    {
        CardListView.ShowCardList(cards);
    }
}
