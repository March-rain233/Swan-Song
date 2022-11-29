using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;
using GameToolKit.Dialog;

internal class ConsumeNode : ProcessNode
{
    [Port("Cost", PortDirection.Input)]
    public int Cost;
    protected override void OnPlay()
    {
        GameManager.Instance.GameData.Gold -= Cost;
        Finish();
    }
}
