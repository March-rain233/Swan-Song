using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class SwingAttack : Card
{
    AreaHelper AreaHelper = new AreaHelper()
    {
        Center = new Vector2Int(0, 1),
        Flags = new bool[2, 3]
        {
            {false, false, false},
            {true, true, true },
        }
    };
    public override CardType Type => CardType.Attack;

    public SwingAttack()
    {
        Name = "挥击";
        Description = "对前方横排三格敌人造成一次<color=red>50%</color>力量值的伤害";
        Cost = 1;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach(var u in GetAffecrTarget(user, target)
            .Where(p=>EnemyFilter(p, user.Camp))
            .Select(p => _map[p].Units.First<IHurtable>()))
        {
            u.Hurt(user.UnitData.Attack * 0.5f, HurtType.FromUnit | HurtType.AD | HurtType.Melee, user);
        }
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData data = new TargetData();
        data.ViewTiles = new List<Vector2Int>();
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Up));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Down));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Left));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Right));
        data.ViewTiles = data.ViewTiles.Where(p =>UniversalFilter(p));
        data.AvaliableTile = data.ViewTiles;
        return data;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AreaHelper.GetPointList(user.Position, (target - user.Position).ToDirection())
            .Where(p=>ExcludeFriendFilter(p, user.Camp));
    }
}
