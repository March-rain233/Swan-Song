using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuffView : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Image ImgBuff;
    public TextMeshProUGUI TxtBuffTime;
    public TextMeshProUGUI TxtBuffLevel;
    public TextMeshProUGUI TxtBuffName;
    public TextMeshProUGUI TxtBuffDescription;
    public GameObject Tip;

    public void Binding(Buff.BuffData buffData)
    {
        ImgBuff.sprite = buffData.Sprite;
        TxtBuffTime.text = buffData.Time > 0 ? $"{buffData.Time}" : "??";
        TxtBuffLevel.text = buffData.Level.ToString();
        TxtBuffDescription.text = buffData.Description;
        TxtBuffName.text = buffData.Name;
        Tip.SetActive(false);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Tip.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Tip.SetActive(false);
    }
}
