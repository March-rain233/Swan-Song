using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Slay : Card
{
    AreaHelper AreaHelper;
    public override CardType Type => CardType.Attack;

    public Slay()
    {
        Name = "斩杀";
        Cost = 3;
        Description = "对身前一格的敌人造成<color=red>200%</color>力量值的伤害，若敌人血量低于40%，直接将其消灭，消灭效果对boss无效";
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        res.Add(target);
        return res;
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData data = new TargetData();
        data.ViewTiles = new List<Vector2Int>();
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Up));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Down));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Left));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Right));
        data.ViewTiles = data.ViewTiles.Where(p=>UniversalFilter(p));
        data.AvaliableTile = data.ViewTiles.Where(p=>EnemyFilter(p, user.Camp));
        return data;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var tar = (_map[target.x, target.y].Units.First() as IHurtable);
        var tb = (tar as Unit).UnitData.Blood;
        var tbmax = (tar as Unit).UnitData.BloodMax;
        if(tb < tbmax * 0.4 && tar is not Boss)
        {
            tar.Hurt(0, HurtType.Death | HurtType.FromUnit | HurtType.AD | HurtType.Melee, user);
        }
        else
        {
            tar.Hurt(user.UnitData.Attack * 2, HurtType.FromUnit, user);
        }

    }
}
