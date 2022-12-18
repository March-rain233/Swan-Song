using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Earthquake : Card
{
    public override CardType Type => CardType.Other;
    public override CardRarity Rarity => CardRarity.Privilege;
    public Earthquake()
    {
        Name = "地震波";
        Description = "令范围内敌人晕眩一回合（对BOSS无效）";
        Cost = 2;
    }

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

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var map = _map;
        var list = AoeArea.GetPointList(target);
        return list.Where(p => ExcludeFriendFilter(p, user.Camp));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = _map.Select(p=>p.pos);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach(var u in GetAffecrTarget(user, target)
            .Where(p=>EnemyFilter(p, user.Camp))
            .Select(p=>_map[p].Units.First())
            .Where(u=>u is not Boss))
        {
            u.AddBuff(new Stun() { Time = 1 });
        }
    }
}
