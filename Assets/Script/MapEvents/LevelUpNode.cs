using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Dialog;
using GameToolKit;

internal class LevelUpNode : ProcessNode
{
    [Port("Index", PortDirection.Input)]
    public int Index;
    protected override void OnPlay()
    {
        GameManager.Instance.GameData.Members[Index].LevelUp();
        Finish();
    }
}
