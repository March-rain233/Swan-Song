using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;
using GameToolKit.Dialog;

internal class MemberDataNode : SourceNode
{
    [Port("Index", PortDirection.Input)]
    public int Index;

    [Port("UnitData", PortDirection.Output, true)]
    public UnitData UnitData;
    protected override void OnValueUpdate()
    {
        UnitData = GameManager.Instance.GameData.Members[Index];
    }
}
