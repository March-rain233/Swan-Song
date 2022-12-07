using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class ExplodeBottle : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(1, 1),
        Flags = new bool[3, 3]
        {
            {true,true,true },
            {true,true,true },
            {true,true,true },
        }
    };

    public ExplodeBottle()//爆爆瓶
    {
        Name = "爆爆瓶";
        Description = "对范围内敌人造成<color=red>200</color>伤害";
        Cost = 3;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AoeArea.GetPointList(target)
            .Where(p=>ExcludeFriendFilter(p, user.Camp));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = _map.Select(p => p.pos);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach (var u in GetAffecrTarget(user, target)
            .Where(p=>_map[p].Units.Count > 0)
            .Select(p=>_map[p].Units.First() as IHurtable))
        {
            u.Hurt(200, HurtType.AP | HurtType.FromUnit | HurtType.Ranged, user);
        }
    }
}
