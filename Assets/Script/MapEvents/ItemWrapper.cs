using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;
using GameToolKit.Dialog;
internal class ItemWrapper : LogicNode
{
    public string ItemName;
    [Port("Cost", PortDirection.Input)]
    public int Cost;
    [Port("Result", PortDirection.Output)]
    public string Result;
    protected override void OnValueUpdate()
    {
        Result = ItemName + $"（{Cost} Gold）";
    }
}

