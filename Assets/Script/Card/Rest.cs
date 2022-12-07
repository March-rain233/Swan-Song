using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Rest : Card
{
    public override CardType Type => CardType.Other;

    public Rest()//休息
    {
        Name="休息";
        Description= "回复一点行动点";
        Cost=0;
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
        targetData.AvaliableTile = new List<Vector2Int>() { user.Position };
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }
    protected internal override void Release(Unit user, Vector2Int target)
    {
        user.UnitData.ActionPoint++;
    }
}
