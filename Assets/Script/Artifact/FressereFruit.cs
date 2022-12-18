using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FressereFruit : Artifact
{
    protected override void OnDisable()
    {
        GameManager.Instance.GameStateChanged -= Instance_GameStateChanged;
    }

    private void Instance_GameStateChanged(GameState obj)
    {
        var sta = obj as BattleState;
        if(sta != null)
        {
            sta.BattleIniting += Sta_BattleIniting;
        }
    }

    private void Sta_BattleIniting()
    {
        var sta = GameManager.Instance.GetState<BattleState>();
        foreach(var player in sta.PlayerList)
        {
            player.Initing += () =>
            {
                player.UnitData.ActionPoint -= 2;
            };
            player.Preparing += () =>
            {
                (player as ICurable).Cure(player.UnitData.BloodMax * 0.1f, this);
            };
        }
    }

    protected override void OnEnable()
    {
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
    }
}
