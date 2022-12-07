using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class SnowStorm : Card
{
    public override CardType Type => CardType.Attack;

    public SnowStorm()//法师专属
    {
        Name = "暴风雪";
        Description = "对周围5x5的方格内角色造成五次60%力量值的伤害，并冻结一回合，使其无法行动";
        Cost = 4;
    }

    public AreaHelper AoeArea = new AreaHelper()
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

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var map = _map;
        var list = AoeArea.GetPointList(user.Position);
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
            && map[p.x, p.y] != null );
        targetData.AvaliableTile = targetData.ViewTiles;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        int times = 1;
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
                            (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * 0.6f, HurtType.FromUnit, user);
                            (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * 0.6f, HurtType.FromUnit, user);
                            (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * 0.6f, HurtType.FromUnit, user);
                            (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * 0.6f, HurtType.FromUnit, user);
                            (tile.Units.First() as IHurtable).Hurt(user.UnitData.Attack * 0.6f, HurtType.FromUnit, user);
                            var tar = (tile.Units.First() as IHurtable);
                            (tar as Unit).CanMove = false;
                        }
                    }
                }
            };
    }
}
