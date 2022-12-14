using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameToolKit;

public class NormalAttack : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AttackArea = new AreaHelper() { Flags = new bool[3, 3]
        {
            { false, true, false },
            { true, false,true },
            { false, true, false }
        },
        Center = new Vector2Int(1, 1)
    };

    public NormalAttack()
    {
        Name = "普通攻击";
        Cost = 1;
        Description = "对敌方造成<color=red>100%</color>力量值的伤害";
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
        var list = AttackArea.GetPointList(user.Position)
            .Where(p=>UniversalFilter(p));
        targetData.ViewTiles = list.Where(p=>UniversalFilter(p));
        targetData.AvaliableTile = list.Where(p => EnemyFilter(p, user.Camp));
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        (_map[target.x, target.y].Units.First() as IHurtable)
            .Hurt(user.UnitData.Attack, HurtType.FromUnit | HurtType.AD | HurtType.Melee, user);
    }
}