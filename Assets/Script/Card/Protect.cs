using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Protect : Card
{
    public override CardType Type => CardType.Heal;

    public Protect()//庇护
    {
        Name = "Protect";
        Description = "Restore blood for characters in range";
        Cost = 2;
    }

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(1, 1),
        Flags = new bool[3, 3]
        {
            {true,true,true },
            {true,true,true },
            {true,true,true }
        }
    };

    public AreaHelper HealArea = new AreaHelper()
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

    public Protect()//庇护
    {
        Name = "Protect";
        Description = "Add blood to enemies in range on two times";
        Cost = 2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var map = _map;
        var list = AoeArea.GetPointList(target);
        list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null);
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var position = user.Position;
        var map = _map;
        var list = HealArea.GetPointList(position);
        targetData.ViewTiles = list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null);
        targetData.AvaliableTile = targetData.ViewTiles.Where(p => map[p.x, p.y].Units.Count > 0);
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        Percent = 0.5f;
        foreach (var point in GetAffecrTarget(user, target))
        {
            if (TileUtility.TryGetTile(point, out var tile))
            {
                if (tile.Units.Count > 0)
                {
                    (tile.Units.First() as ICurable).Cure(user.UnitData.Heal * Percent + 50, user);
                }
            }
        }
        Times++;
    }
}
