using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class StabAttack : Card
{
    public AreaHelper AreaHelper = new AreaHelper()
    {
        Center = new Vector2Int(0, 0),
        Flags = new bool[4, 1]
        {
            {false },
            {true},
            {true},
            {true }
        }
    };

    public override CardType Type => CardType.Attack;

    public StabAttack()
    {
        Name = "穿刺";
        Description = "对前方三格敌人造成一次<color=red>70%</color>力量值的伤害";
        Cost = 2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AreaHelper.GetPointList(user.Position, (target - user.Position).ToDirection())
            .Where(p => ExcludeFriendFilter(p, user.Camp));
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

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach (var u in GetAffecrTarget(user, target)
            .Where(p => EnemyFilter(p, user.Camp))
            .Select(p => _map[p.x, p.y].Units.First() as IHurtable))
        {
            u.Hurt(user.UnitData.Attack * 0.7f, HurtType.AD | HurtType.FromUnit, user);
        }
    }
}
