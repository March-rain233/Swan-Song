using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Dialog;
using GameToolKit;

internal class BattleNode : ProcessNode
{
    protected override void OnPlay()
    {
        var gm = GameManager.Instance;
        gm.SetStatus<BattleState>()
            .InitSystem(gm.GameData.Chapter, 2);
    }
}

