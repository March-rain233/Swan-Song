using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class FireRain : Card
{
    public override CardType Type => CardType.Attack;

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

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(2, 2),
        Flags = new bool[5, 5]
    {
            { true, true, true, true, false},
            { true, true, true, true, false},
            { true, true, true, true, false},
            { true, true, true, true, false},
            { false, false, false, false, false}
    }
    };

    public FireRain()//火雨_法师专属
    {
        Name="火雨";
        Description= "选定一个4x4的方格，对其中敌人造成150%力量值的伤害，并使其获得三回合灼伤效果";
        Cost=3;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var map = _map;
        var list = AoeArea.GetPointList(target);
        return list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null
            && _map[p.x, p.y].Units.First().Camp != user.Camp);
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
        int times = 3;
        foreach (var point in GetAffecrTarget(user, target))
        {
            if (TileUtility.TryGetTile(point, out var tile))
            {
                if (tile.Units.Count > 0 && tile.Units.First().Camp != user.Camp)
                {
                    (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * 1.5f, HurtType.FromUnit, user);
                }
            }
        }
        GameManager.Instance.GetState<BattleState>()
            .TurnBeginning += (_) =>
            {
                times -= 1;
                if (times >= 0)
                {
                    foreach (var point in GetAffecrTarget(user, target))
                    {
                        if (TileUtility.TryGetTile(point, out var tile))
                        {
                            if (tile.Units.Count > 0 && tile.Units.First().Camp != user.Camp)
                            {
                                (tile.Units.First() as IHurtable).Hurt(user.UnitData.Blood * 0.2f, HurtType.FromUnit, user);
                            }
                            if (tile.TileType != TileType.Lack)
                            {
                                tile.AddStatus(TileStatus.Fire);
                            }
                        }
                    }
                }
            };
    }
}
