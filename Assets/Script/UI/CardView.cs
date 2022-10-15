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
    IArcLayoutElement,
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
    public CardScheduler CardScheduler;

    public event System.Action MouseEntered;
    public event System.Action MouseExited;
    public event System.Action MouseUp;
    public event System.Action MouseDown;
    public event System.Action MouseMove;

    [OdinSerialize]
    public float Angle { get; set; }
    [OdinSerialize]
    public float PreferAngle { get; set; } = 20;
    [OdinSerialize]
    public float MinnumAngle { get; set; } = 8;
    [OdinSerialize]
    public float FlexibleAngle { get; set; }

    public void SetPosition(Vector2 position)
    {
        transform.DOLocalMove(position, 0.1f);
    }

    public void SetRotation(float eulerAngle)
    {
        transform.DOLocalRotate(new Vector3(0, 0, eulerAngle - 90), 0.1f);
    }

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
