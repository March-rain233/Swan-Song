using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;
using GameToolKit.Dialog;

internal class GameDataNode : SourceNode
{
    [Port("Source", PortDirection.Output, true)]
    public GameData GameData;
    protected override void OnValueUpdate()
    {
        GameData = GameManager.Instance.GameData;
    }
}

