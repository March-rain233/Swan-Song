using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameToolKit;

public class UnitDataView:MonoBehaviour
{
    public UnitData Unit;

    public Image Face;
    public TextMeshProUGUI Name;
    public Slider SldBlood;
    public TextMeshProUGUI Blood;
    public void BindingUnit(UnitData unit)
    {
        if (Unit != null)
        {
            Unit.DataChanged -= Refresh;
        }
        Unit = unit;
        Unit.DataChanged += Refresh;
        Refresh();
    }

    void Refresh()
    {
        Face.sprite = Unit.Face;
        Name.text = Unit.Name;
        SldBlood.value = Unit.Blood / (float)Unit.BloodMax;
        Blood.text = $"{Unit.Blood}/{Unit.BloodMax}";
    }
}
