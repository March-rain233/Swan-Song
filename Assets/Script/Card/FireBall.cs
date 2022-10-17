using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireBall : Card
{
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
    protected internal override IEnumerable<Vector2Int> GetActionRange(Unit user, Vector2Int target)
    {
        var map = _map;
        var list = AttackArea.GetPointList(target);
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
        throw new System.NotImplementedException();
    }
}