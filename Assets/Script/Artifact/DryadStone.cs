using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DryadStone : Artifact
{
    protected override void OnDisable()
    {
        GameManager.Instance.GameStateChanged -= Instance_GameStateChanged;
    }

    private void Instance_GameStateChanged(GameState obj)
    {
        var sta = obj as BattleState;
        if (sta != null)
        {
            sta.BattleIniting += Sta_BattleIniting;
        }
    }

    private void Sta_BattleIniting()
    {
        var sta = GameManager.Instance.GetState<BattleState>();
        foreach (var player in sta.PlayerList)
        {
            player.AddBuff(new DryadShield() { Level = 2 });
        }
    }

    protected override void OnEnable()
    {
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
    }
}

