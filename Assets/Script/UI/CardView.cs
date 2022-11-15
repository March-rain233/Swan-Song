using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardView : SerializedMonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerMoveHandler
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI AP;
    public Image Sprite;

    public Card Card;

    public event System.Action MouseEntered;
    public event System.Action MouseExited;
    public event System.Action MouseUp;
    public event System.Action MouseDown;
    public event System.Action MouseMove;

    public void Refresh()
    {
        Name.text = Card.Name;
        Description.text = Card.Description;
        AP.text = Card.Cost.ToString();
        Sprite.sprite = Card.Sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseEntered?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseExited?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MouseUp?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MouseDown?.Invoke();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        MouseMove?.Invoke();
    }
}
