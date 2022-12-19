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
    public override CardRarity Rarity => CardRarity.Privilege;
    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(1, 1),
        Flags = new bool[5, 5]
    {
            { true, true, true, true, true},
            { true, true, true, true, true},
            { true, true, true, true, true},
            { true, true, true, true, true},
            { true, true, true, true, true},
    }
    };

    public FireRain()//火雨_法师专属
    {
        Name="火雨";
        Description= "对范围内敌人造成自身<color=red>150%</color>力量值的伤害，并使其获得三回合灼伤效果";
        Cost=3;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AoeArea.GetPointList(target)
            .Where(p =>ExcludeFriendFilter(p, user.Camp));
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
            .Where(p => EnemyFilter(p, user.Camp))
            .Select(p => _map[p].Units.First()))
        {
            (u as IHurtable).Hurt(user.UnitData.Attack * 1.5f, HurtType.AP | HurtType.FromUnit | HurtType.Ranged, user);
            u.AddBuff(new Burn() { Time = 3 });
        }
    }
}
