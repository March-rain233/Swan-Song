using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class IronThornPlant : Card
{
    public override CardType Type => CardType.Other;

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(1, 1),
        Flags = new bool[3, 3]
        {
            { true, true, true },
            { true, false, true },
            { true, true, true }
        }
    };

    public IronThornPlant()
    {
        Name = "铁蒺藜";
        Cost = 2;
        Description = "在周围八格布置一圈铁蒺藜，持续两回合，对触碰的敌人造成<color=red>30%</color>力量值的伤害";
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AoeArea.GetPointList(user.Position)
            .Where(p=>UniversalFilter(p));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        targetData.ViewTiles = new List<Vector2Int>() { user.Position };
        targetData.AvaliableTile = targetData.ViewTiles;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach(var tile in GetAffecrTarget(user, target)
            .Select(t => _map[t]))
        {
            tile.AddStatus(new CaltropStatus() { Damage = user.UnitData.Attack * 0.3f , Times = 2});
        }
    }
}
