using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class EarthShock : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AttackArea = new AreaHelper()
    {
        Flags = new bool[4, 4]
        {
            { true, true, true, true },
            { true, true, true, true },
            { true, true, true, true },
            { true, true, true, true }
        },
        Center = new Vector2Int(2, 2)
    };

    public EarthShock()
    {
        Name = "震地击";
        Description = "对范围内敌人造成<color=red>150%</color>力量值的伤害，并使其中的敌人晕眩一回合";
        Cost = 2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AttackArea.GetPointList(user.Position)
            .Where(p => ExcludeFriendFilter(p, user.Camp));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = AttackArea.GetPointList(user.Position);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach (var u in GetAffecrTarget(user, target)
            .Where(p => EnemyFilter(p, user.Camp))
            .Select(p => _map[p].Units.First()))
        {
            (u as IHurtable)
            .Hurt(user.UnitData.Attack * 1.5f, HurtType.FromUnit | HurtType.AD | HurtType.Melee, user);
            u.AddBuff(new Stun() { Time = 1 });
        }
    }
}
