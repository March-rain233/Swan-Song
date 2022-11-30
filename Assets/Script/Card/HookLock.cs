using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class HookLock : Card
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

    public HookLock()//钩锁
    {
        Name = "Hook Lock";
        Cost = 2;
        Description = "Pull an enemy in front of you and deal damage";
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        res.Add(target);
        return res;
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
            && map[p.x, p.y] != null
            && map[p.x, p.y].Units.First().Camp != user.Camp);
        targetData.AvaliableTile = targetData.ViewTiles.Where(p => map[p.x, p.y].Units.Count > 0);
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        Percent = 0.2f;
        var tar = (_map[target.x, target.y].Units.First() as IHurtable);
        tar.Hurt(user.UnitData.Attack * Percent, HurtType.FromUnit, user);
        var tp = (tar as Unit).Position;
        Vector2Int dir = (tp - user.Position).ToDirection().ToVector2Int();
        (tar as Unit).Position = user.Position + dir;
    }
}
