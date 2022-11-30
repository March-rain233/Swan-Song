using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class ManaNerf : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(2, 2),
        Flags = new bool[5, 5]
        {
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true },
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

    public ManaNerf()
    {
        Name = "魔力削弱";
        Description = "对范围内的敌人造成30%力量值的伤害，并使其在本回合内防御减少30点";
        Cost = 3;
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
        Percent = 0.3f;
        int times = 1;
        GameManager.Instance.GetState<BattleState>()
            .TurnBeginning += (_) =>
            {
                times -= 1;
                foreach (var point in GetAffecrTarget(user, target))
                {
                    if (TileUtility.TryGetTile(point, out var tile))
                    {
                        if (tile.Units.Count > 0 && times == 0)
                        {
                            var tar = (tile.Units.First() as IHurtable);
                            (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * Percent, HurtType.FromUnit, user);
                            (tar as Unit).UnitData.Defence = (tar as Unit).UnitData.Defence - 30;
                        }
                    }
                }
            };
    }
}
