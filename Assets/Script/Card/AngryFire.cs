using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class AngryFire : Card
{
    public override CardType Type => CardType.Other;
    public AngryFire()
    {
        Name = "怒火";
        Description = "本回合受到的伤害减少<color=blue>30%</color>" +
            "增加<color=red>50</color>点力量值" +
            "若血量低于70%，额外抽取一张卡牌";
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
        targetData.AvaliableTile = new List<Vector2Int>() { user.Position };
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        _map[target].Units.First().AddBuff(new Angry() { Time = 1 });
        if (user.UnitData.Blood < user.UnitData.BloodMax * 0.7)
            user.Scheduler.DrawCard();
    }
}
