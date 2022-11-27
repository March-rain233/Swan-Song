using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class SwingAttack : Card
{
    AreaHelper AreaHelper;
    public override CardType Type => CardType.Attack;

    public SwingAttack()
    {
        Name = "挥击";
        Description = "对前方横排三格敌人造成一次50%力量值的伤害";
        Cost = 1;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        Percent = 0.5f;
        var list = GetAffecrTarget(user, target)
            .Where(p => _map[p.x, p.y].Units.Count > 0)
            .Select(p => _map[p.x, p.y].Units.First());
        foreach(IHurtable u in list)
        {
            u.Hurt(user.UnitData.Attack * Percent, HurtType.AD | HurtType.FromUnit, user);
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
        data.ViewTiles = data.ViewTiles.Where(p =>
            0 <= p.x && p.x < _map.Width
            && 0 <= p.y && p.y < _map.Height
            && _map[p.x, p.y] != null);
        data.AvaliableTile = data.ViewTiles;
        return data;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var dir = (target - user.Position).ToDirection();
        return AreaHelper.GetPointList(user.Position, dir)
            .Where(p =>
            0 <= p.x && p.x < _map.Width
            && 0 <= p.y && p.y < _map.Height
            && _map[p.x, p.y] != null);
    }
}
