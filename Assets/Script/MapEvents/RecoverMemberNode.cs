using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Dialog;
using GameToolKit;
using UnityEngine;

internal class RecoverMemberNode : ProcessNode
{
    [Port("Index", PortDirection.Input)]
    public int Index;
    public float Percent;
    protected override void OnPlay()
    {
        var m = GameManager.Instance.GameData.Members[Index];
        m.Blood = Mathf.Min(m.Blood + Mathf.CeilToInt(m.BloodMax * Percent), m.BloodMax);
        Finish();
    }
}
