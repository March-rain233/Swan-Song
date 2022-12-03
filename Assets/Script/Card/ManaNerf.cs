using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class ManaNerf : Card
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
            {true,true,true,true,true },
        }
    };

    public ManaNerf()
    {
        Name = "魔力削弱";
        Description = "对范围内的敌人造成<color=red>30%</color>力量值的伤害，并使其在本回合内防御减少30点";
        Cost = 3;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AoeArea.GetPointList(target)
            .Where(p => ExcludeFriendFilter(p, user.Camp));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = _map.Select(p=>p.pos);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = targetData.ViewTiles;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach (var u in GetAffecrTarget(user, target)
            .Where(p => EnemyFilter(p, user.Camp))
            .Select(p => _map[p].Units.First()))
        {
            (u as IHurtable).Hurt(user.UnitData.Attack * 0.3f, HurtType.FromUnit | HurtType.AP | HurtType.Ranged, user);
            u.AddBuff(new ArcaneChain() { Time = 1 });
        }
    }
}
