using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Dedicate : Card
{
    public override CardType Type => CardType.Heal;

    public Dedicate()
    {
        Name = "奉献";
        Description = "自己扣除10%最大生命值，让指定目标恢复他的20%最大生命值";
        Cost = 1;
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
            .Where(u => u.Camp == user.Camp && u.ActionStatus != ActionStatus.Dead)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        user.UnitData.Blood = user.UnitData.Blood - user.UnitData.BloodMax/10;
        Percent = 0.2f;
        (_map[target.x, target.y].Units.First() as ICurable)
            .Cure(user.UnitData.Heal * Percent , user);
    }
}
