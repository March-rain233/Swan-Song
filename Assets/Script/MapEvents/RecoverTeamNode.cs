using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Dialog;
using GameToolKit;
using UnityEngine;

internal class RecoverTeamNode : ProcessNode
{
    public float Percent;
    protected override void OnPlay()
    {
        foreach(var m in GameManager.Instance.GameData.Members)
        {
            m.Blood = Mathf.Min(m.Blood + Mathf.FloorToInt(m.BloodMax * Percent), m.BloodMax);
        }
        Finish();
    }
}
