using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class CardView : SerializedMonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerMoveHandler,
    IPointerClickHandler
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI AP;
    public TextMeshProUGUI OwnerName;

    public Image Sprite;

    public Card Card;
    public CardScheduler CardScheduler;

    public event Action MouseEntered;
    public event Action MouseExited;
    public event Action MouseUp;
    public event Action MouseDown;
    public event Action MouseMove;
    public event Action Clicked;

    public void Refresh()
    {
        Name.text = Card.Name;
        Description.text = Card.Description;
        AP.text = Card.Cost.ToString();
        Sprite.sprite = Card.Sprite;
        OwnerName.text = CardScheduler.Unit.UnitData.Name;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        MouseEntered?.Invoke();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        MouseExited?.Invoke();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        MouseUp?.Invoke();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        MouseDown?.Invoke();
    }

    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
    {
        MouseMove?.Invoke();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke();
    }
}
