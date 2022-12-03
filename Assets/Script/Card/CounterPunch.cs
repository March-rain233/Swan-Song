using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class CounterPunch : Card
{
    public override CardType Type => CardType.Other;

    public CounterPunch()//反击重拳
    {
        Name = "反击重拳";
        Description = "下次遭遇敌人的近身攻击时，对其造成<color=red>100%</color>力量值的伤害";
        Cost = 1;
    }
    protected internal override void Release(Unit user, Vector2Int target)
    {
        user.AddBuff(new Endure());
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        return new TargetData()
        {
            AvaliableTile = new List<Vector2Int>() { user.Position },
            ViewTiles = new List<Vector2Int>() { user.Position },
        };
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return new List<Vector2Int>() { user.Position };
    }
}
