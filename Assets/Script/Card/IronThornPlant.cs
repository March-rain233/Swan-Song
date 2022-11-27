using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class IronThornPlant : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(1, 1),
        Flags = new bool[3, 3]
        {
            { true, true, true },
            { true, true, true },
            { true, true, true }
        }
    };

    public AreaHelper AttackArea = new AreaHelper()
    {
        Flags = new bool[3, 3]
        {
            { false, true, false },
            { true, false,true },
            { false, true, false }
        },
        Center = new Vector2Int(1, 1)
    };

    public IronThornPlant()
    {
        Name = "铁蒺藜";
        Cost = 2;
        Description = "在周围八格布置一圈铁蒺藜，持续两回合，对触碰的敌人造成30%力量值的伤害";
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var map = _map;
        var list = AoeArea.GetPointList(user.Position);
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
        Percent = 0.3f;
        int times = 2;
        GameManager.Instance.GetState<BattleState>()
            .TurnBeginning += (_) =>
            {
                times -= 1;
                foreach (var point in GetAffecrTarget(user, target))
                {
                    if (TileUtility.TryGetTile(point, out var tile))
                    {
                        if (tile.Units.Count > 0 && times >= 0)
                        {
                            (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * Percent, HurtType.FromUnit, user);

                        }
                    }
                }
            };
    }
}
