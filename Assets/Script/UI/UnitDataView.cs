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
    public HpBar HpBar;
    public void BindingUnit(UnitData unit)
    {
        if (Unit != null)
        {
            Unit.DataChanged -= Refresh;
        }
        Unit = unit;
        Unit.DataChanged += Refresh;
        HpBar.InitHpBar(Unit.Blood, Unit.BloodMax);
        Refresh();
    }

    void Refresh()
    {
        Face.sprite = Unit.Face;
        Name.text = Unit.Name;
        HpBar.MaxHp = Unit.BloodMax;
        HpBar.Hp = Unit.Blood;
    }
}
