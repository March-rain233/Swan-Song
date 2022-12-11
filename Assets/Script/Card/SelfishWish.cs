using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class SelfishWish : Card
{
    public override CardType Type => CardType.Other;

    public SelfishWish()
    {
        Name = "自私的祝福";
        Description = "选定一个友方角色，在本局对战中，此角色每造成一次伤害，回复施法者<color=green>20</color>点生命";
        Cost = 2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        res.Add(target);
        return res;
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        var targetData = new TargetData();
        targetData.AvaliableTile = GetUnitList()
            .Where(u => u.Camp == user.Camp)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        _map[target].Units.First()
            .AddBuff(new SoulLink() { User = user });
    }
}
