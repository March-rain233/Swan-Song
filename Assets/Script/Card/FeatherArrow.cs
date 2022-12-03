using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FeatherArrow : Card
{
    public override CardType Type => CardType.Attack;

    public FeatherArrow()//穿羽箭_弓手专属
    {
        Name="穿羽箭";
        Description= "造成<color=red>200%</color>力量值的伤害，无视防御力";
        Cost=2;
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
        var list = _map.Select(p => p.pos);
        targetData.AvaliableTile = list.Where(p => EnemyFilter(p, user.Camp));
        targetData.ViewTiles = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        (_map[target.x, target.y].Units.First() as IHurtable)
            .Hurt(user.UnitData.Attack * 2, HurtType.FromUnit | HurtType.Ranged | HurtType.AD | HurtType.APDS, user);
    }
}
