using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class HeroSoul : Artifact
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
            sta.TurnEnding += _ =>
            {
                (player as IHurtable).Hurt(player.UnitData.BloodMax * 0.15f, HurtType.FromBuff, this);
            };
            player.Preparing += () =>
            {
                player.UnitData.ActionPoint += 1;
            };
            player.Scheduler.HandsRemoved += _ =>
            {
                if(player.Scheduler.Hands.Count == 0)
                {
                    player.Scheduler.DrawCard();
                }
            };
        }
    }

    protected override void OnEnable()
    {
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
    }
}
