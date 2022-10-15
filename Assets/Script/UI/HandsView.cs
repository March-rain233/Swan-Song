using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using GameToolKit;

/// <summary>
///  ÷≈∆ ”Õº
/// </summary>
public class HandsView : SerializedMonoBehaviour
{
    public ArcLayout ArcLayout;
    public float DefaultMinnumAngle;
    public float DefaultPreferedAngle;
    public float FloatPreferedAngle;

    CardView _select;
    public void AddCard(Card card, CardScheduler scheduler)
    {
        CardView cardView = Instantiate(UISetting.Instance.PrefabsDic["Card"], transform)
            .GetComponent<CardView>(); 

        cardView.Card = card;
        cardView.CardScheduler = scheduler;
        cardView.MinnumAngle = DefaultMinnumAngle;
        cardView.PreferAngle = DefaultPreferedAngle;
        cardView.MouseEntered += () =>
        {
            cardView.PreferAngle = FloatPreferedAngle;
            ArcLayout.Refresh();
        };
        cardView.MouseExited += () =>
        {
            cardView.PreferAngle = DefaultPreferedAngle;
            ArcLayout.Refresh();
        };
        cardView.MouseDown += () =>
        {
            Debug.Log(0);
            cardView.transform.DOBlendableLocalMoveBy(new Vector3(0, 30, 0), 0.3f);
            cardView.transform.DOLocalRotate(Vector3.zero, 0.1f);
        };
        cardView.MouseUp += () =>
        {
            Debug.Log(1);
            ArcLayout.Refresh();
        };
        cardView.MouseMove += () =>
        {
            Debug.Log(3);
        };
        cardView.Refresh();

        ArcLayout.Children.Add(cardView);
        ArcLayout.Refresh();
    }

    void SelectCard(CardView cardView)
    {
        _select = cardView;
    }

    void UnSelect()
    {

    }
}
