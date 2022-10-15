using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSelectView : MonoBehaviour
{
    public UnitData UnitData;

    public GameObject JoinFlag;
    public Image Face;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI BloodText;
    public Slider Blood;
    public Image Background;
    public Toggle Toggle;
    public Shader BorderShader;

    public Outline Outline;

    public void Binding(UnitData unit)
    {
        UnitData = unit;
        Face.sprite = UnitData.Face;
        Name.text = UnitData.Name;
        Blood.value = UnitData.Blood / UnitData.BloodMax;
        BloodText.text = $"{UnitData.Blood}/{UnitData.BloodMax}";
        JoinFlag.SetActive(false);
        Outline.enabled = false;
        Toggle.onValueChanged.AddListener(value =>
        {
            if (value)
            {
                Outline.enabled = true;
            }
            else
            {
                Outline.enabled = false;
            }
        });
    }

    public void Join()
    {
        JoinFlag.SetActive(true);
    }

    public void Unjoin()
    {
        JoinFlag.SetActive(false);
    }
}
