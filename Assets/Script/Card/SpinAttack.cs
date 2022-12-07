using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class SpinAttack : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AttackArea = new AreaHelper()
    {
        Flags = new bool[3, 3]
        {
            { true, true, true },
            { true, false,true },
            { true, true, true }
        },
        Center = new Vector2Int(1, 1)
    };

    public SpinAttack()
    {
        Name="回旋斩";
        Description= "对周围敌人造成一次<color=red>50%</color>力量值的伤害";
        Cost=2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AttackArea.GetPointList(user.Position)
            .Where(p=>ExcludeFriendFilter(p, user.Camp));
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
            .Where(p=>EnemyFilter(p, user.Camp))
            .Select(p=>_map[p].Units.First() as IHurtable))
        {
            u.Hurt(user.UnitData.Attack * 0.5f, HurtType.FromUnit | HurtType.AD | HurtType.Melee, user);
        }
    }
}
