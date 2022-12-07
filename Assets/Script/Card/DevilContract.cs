using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class DevilContract : Card
{
    public override CardType Type => CardType.Other;

    public DevilContract()
    {
        Name = "恶魔契约";
        Description = "五回合内受到伤害减少<color=blue>50%</color>，" +
            "造成的伤害增加<color=red>20%</color>，" +
            "每回合额外回复<color=yellow>1</color>点体力，<color=purple>5</color>回合后立刻死亡";
        Cost = 4;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        res.Add(target);
        return res;
    }
    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        var targetData = new TargetData();
        targetData.AvaliableTile = GameManager.Instance.GetState<BattleState>().UnitList
            .Where(u => u.Camp == user.Camp && u.ActionStatus != ActionStatus.Dead && u.Position == user.Position)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        user.AddBuff(new DaemonBride() { Time = 5 });
    }
}
