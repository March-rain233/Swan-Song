using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Ambush : Card
{
    public override CardType Type => CardType.Other;
    public Card CardPointer;

    public Ambush()
    {
        Name = "埋伏";
        Description = "将两张挥击置入手牌";
        Cost = 1;
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
        user.Scheduler.AddToHand(CardPointer.Clone());
        user.Scheduler.AddToHand(CardPointer.Clone());
    }
}
