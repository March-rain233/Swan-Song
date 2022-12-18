using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ShieldOfBrambles : Artifact
{
    protected override void OnDisable()
    {
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
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
            player.Hurt += (damage, type, obj) =>
            {
                if (type.HasFlag(HurtType.FromUnit))
                {
                    (obj as IHurtable).Hurt(damage * 0.2f, HurtType.FromBuff | HurtType.APDS, this);
                }
            };
        }
    }

    protected override void OnEnable()
    {
        GameManager.Instance.GameStateChanged -= Instance_GameStateChanged;
    }
}

