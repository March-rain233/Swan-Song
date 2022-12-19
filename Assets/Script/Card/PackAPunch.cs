using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class PackAPunch : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(2, 2),
        Flags = new bool[5, 5]
        {
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true }
        }
    };

    public PackAPunch ()//蓄力猛击
    {
        Name = "蓄力猛击";
        Description = "对范围内敌人造成<color=red>10%</color>力量值的伤害，每消耗1体力，就额外造成<color=red>40%</color>力量值的伤害";
        Cost = -1;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AoeArea.GetPointList(user.Position)
            .Where(p =>ExcludeFriendFilter(p, user.Camp));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = _map.Select(p=>p.pos);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var damage = user.UnitData.Attack * (0.1f + 0.4f * user.UnitData.ActionPoint);
        foreach (var u in GetAffecrTarget(user, target)
            .Where(p=>EnemyFilter(p, user.Camp))
            .Select(p=>_map[p].Units.First() as IHurtable))
        {
            u.Hurt(damage, HurtType.FromUnit | HurtType.AD, user);
        }
    }
}
