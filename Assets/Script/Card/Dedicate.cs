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
        Description = "对自身造成等于<color=red>10%</color>最大生命值的魔法伤害，恢复选中目标<color=green>20%</color>最大生命值的血量";
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
        targetData.AvaliableTile = GetUnitList()
            .Where(u => u.Camp == user.Camp)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        (user as IHurtable).Hurt(user.UnitData.BloodMax * 0.1f, HurtType.FromUnit | HurtType.AP, user);
        (_map[target].Units.First() as ICurable)
            .Cure(user.UnitData.BloodMax * 0.2f , user);
    }
}
