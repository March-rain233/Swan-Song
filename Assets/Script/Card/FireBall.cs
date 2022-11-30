using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireBall : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AoeArea = new AreaHelper() { Center = new Vector2Int(2, 2), Flags = new bool[5, 5]
        {
            {false,false,true,false,false },
            {false,true,true,true,false },
            {true,true,true,true,true },
            {false,true,true,true,false },
            {false,false,true,false,false }
        }
    };
    public AreaHelper AttackArea = new AreaHelper()
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
    public FireBall()
    {
        Name = "Fire Ball";
        Description = "AOE";
        Cost = 5;
    }
    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var map = _map;
        var list = AoeArea.GetPointList(target);
        return list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null
            && map[p.x, p.y].Units.First().Camp != user.Camp);
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var position = user.Position;
        var map = _map;
        var list = AttackArea.GetPointList(position);
        targetData.ViewTiles = list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null);
        targetData.AvaliableTile = targetData.ViewTiles;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach(var point in GetAffecrTarget(user, target))
        {
            if(TileUtility.TryGetTile(point, out var tile))
            {
                if (tile.Units.Count > 0 && tile.Units.First().Camp != user.Camp)
                {
                    (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * 2, HurtType.FromUnit, user);
                }
                if (tile.TileType != TileType.Lack)
                {
                    tile.AddStatus(TileStatus.Fire);
                }
            }
        }
    }
}
