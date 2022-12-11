using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class HookLock : Card
{
    public override CardType Type => CardType.Attack;

    public HookLock()//钩锁
    {
        Name = "钩锁";
        Cost = 2;
        Description = "对目标造成自身<color=red>20%</color>力量值伤害，并将其拉至身前一格";
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        res.Add(target);
        return res;
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = GetUnitList()
            .Select(p => p.Position)
            .Where(p => CheckPlaceable(user.Position + (p - user.Position).ToDirection().ToVector2Int(), user));
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var tar = _map[target.x, target.y].Units.First();
        (tar as IHurtable).Hurt(user.UnitData.Attack * 0.2f, HurtType.FromUnit | HurtType.AD | HurtType.Ranged, user);
        tar.Position = user.Position + (tar.Position - user.Position).ToDirection().ToVector2Int();
    }
}
