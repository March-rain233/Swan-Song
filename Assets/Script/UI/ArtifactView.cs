using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class ArtifactView : SerializedMonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    IArcLayoutElement
{
    public Image ImgArtifact;
    public TextMeshProUGUI TxtName;
    public TextMeshProUGUI TxtDescription;
    public GameObject Tip;

    [OdinSerialize]
    public float Angle {get; set;}
    [OdinSerialize]
    public float PreferAngle { get; set; }
    [OdinSerialize]
    public float MinnumAngle { get; set; }
    [OdinSerialize]
    public float FlexibleAngle { get; set; }

    public event Action Clicked;

    public void Binding(Artifact artifact)
    {
        ImgArtifact.sprite = artifact.Sprite;
        TxtDescription.text = artifact.Description;
        TxtName.text = artifact.Name;
        Tip.SetActive(false);
    }

    public void SetPosition(Vector2 position)
    {
        transform.localPosition = position;
    }

    public void SetRotation(float eulerAngle)
    {
        
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke();
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

