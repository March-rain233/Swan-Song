using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Pray : Card
{
    public override CardType Type => CardType.Other;

    public Pray()
    {
        Name = "祈祷";
        Description = "抽两张牌，如果手牌数量少于3张，则改为抽取三张";
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
        targetData.AvaliableTile = GameManager.Instance.GetState<BattleState>().UnitList
            .Where(u => u.Camp == user.Camp && u.ActionStatus != ActionStatus.Dead && u.Position == user.Position)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        if(user.Scheduler.Hands.Count < 3)
        {
            user.Scheduler.DrawCard();
            user.Scheduler.DrawCard();
            user.Scheduler.DrawCard();
        }
        else
        {
            user.Scheduler.DrawCard();
            user.Scheduler.DrawCard();
        }
    }
}
