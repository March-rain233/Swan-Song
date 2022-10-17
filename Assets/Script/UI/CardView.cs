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

    public LineRenderer LineRenderer;
    public Outline Outline;

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

    public void SetLineEnd(Vector2 endPos)
    {
        Vector2 startPos = Vector2.zero;

        Vector2 ctrlAPos = new Vector2();
        Vector2 ctrlBPos = new Vector2();
        ctrlAPos.x = startPos.x + (startPos.x - endPos.x) * 0.1f;
        ctrlAPos.y = endPos.y - (endPos.y - startPos.y) * 0.2f;
        ctrlBPos.y = endPos.y + (endPos.y - startPos.y) * 0.3f;
        ctrlBPos.x = startPos.x - (startPos.x - endPos.x) * 0.3f;

        Vector3[] positions = new Vector3[LineRenderer.positionCount];
        LineRenderer.GetPositions(positions);
        for (int i = 0; i < positions.Length; ++i)
        {
            float t = (i / (float)(positions.Length - 1));
            positions[i] = startPos * (1 - t) * (1 - t) * (1 - t) + 3 * ctrlAPos * t * (1 - t) * (1 - t) + 3 * ctrlBPos * t * t * (1 - t) + endPos * t * t * t;
        }

        LineRenderer.SetPositions(positions);
    }

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
