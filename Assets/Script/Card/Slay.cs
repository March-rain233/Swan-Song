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
        Description = "对身前一格的敌人造成200%力量值的伤害，若敌人血量低于40%，直接将其消灭，消灭效果对boss无效";
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
        data.ViewTiles = data.ViewTiles.Where(p =>
            0 <= p.x && p.x < _map.Width
            && 0 <= p.y && p.y < _map.Height
            && _map[p.x, p.y] != null);
        data.AvaliableTile = data.ViewTiles;
        return data;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        Percent = 2;
        (_map[target.x, target.y].Units.First() as IHurtable)
            .Hurt(user.UnitData.Attack * Percent, HurtType.FromUnit, user);
    }
}
