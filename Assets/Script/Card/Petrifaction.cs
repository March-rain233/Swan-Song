using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Petrifaction : Card
{
    public override CardType Type => CardType.Other;
    public Petrifaction(Unit user)
    {
        Name = "石化";
        Description = "接下来两回合受到的伤害减少<color=blue>30%</color>，同时不能移动";
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
        TargetData targetData = new TargetData();
        var list = GameManager.Instance.GetState<BattleState>().UnitList
            .Select(u => u.Position);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        _map[target].Units.First().AddBuff(new Petrify());
    }
}
