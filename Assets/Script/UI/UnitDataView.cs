using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameToolKit;

public class UnitDataView:MonoBehaviour
{
    #region UI¿Ø¼þ
    public Image Face;
    public TextMeshProUGUI Name;
    public HpBar HpBar;
    #endregion

    public void Refresh(UnitData data)
    {
        Face.sprite = data.Face;
        Name.text = data.Name;
        HpBar.MaxHp = data.BloodMax;
        HpBar.Hp = data.Blood;
    }

    public void RefreshWithoutAnim(UnitData data)
    {
        Face.sprite = data.Face;
        Name.text = data.Name;
        HpBar.InitHpBar(data.Blood, data.BloodMax);
    }
}
