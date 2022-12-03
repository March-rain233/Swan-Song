using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class WaterAttack : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(0, 0),
        Flags = new bool[2, 2]
        {
            { true, true },
            { true, true},
        }
    };

    public WaterAttack()//水术弹
    {
        Name= "水弹术";
        Description= "对范围内的敌人造成<color=red>25%</color>力量值的伤害";
        Cost=1;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AoeArea.GetPointList(target)
            .Where(p =>ExcludeFriendFilter(p, user.Camp));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = _map.Select(p=>p.pos);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list ;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach (var u in GetAffecrTarget(user, target)
            .Where(p=>EnemyFilter(p, user.Camp))
            .Select(p=>_map[p].Units.First<IHurtable>()))
        {
            u.Hurt(user.UnitData.Attack * 0.25f, HurtType.AP | HurtType.Ranged | HurtType.FromUnit, user);
        }
    }

}
