using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class BloodAttack : Card
{
    public override CardType Type => CardType.Other;

    public BloodAttack()
    {
        Name = "嗜血";
        Description = "扣除10%最大生命值，在接下来的三回合内，力量值增加100点";
        Cost = 2;
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
        user.AddBuff(new Bloodlust() { Count = 3 });
    }
}
