using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class ForceStorageMatrix : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(2, 2),
        Flags = new bool[4, 4]
    {
            { true, true, true, true },
            { true, true, true, true },
            { true, true, true, true },
            { true, true, true, true }
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

    public ForceStorageMatrix()//蓄力法阵
    {
        Name = "Force Storage Matrix";
        Description = "Deal huge damage to range enemies after two times";
        Cost = 2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var map = _map;
        var list = AoeArea.GetPointList(target);
        return list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null);
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
        if(Times == 2)
        {
            foreach (var point in GetAffecrTarget(user, target))
            {
                if (TileUtility.TryGetTile(point, out var tile))
                {
                    if (tile.Units.Count > 0)
                    {
                        (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * 2, HurtType.FromUnit, user);
                    }
                }
            }
        }
        Times++;
    }
}
