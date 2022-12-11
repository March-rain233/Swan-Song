using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Curse : Card
{
    public override CardType Type => CardType.Other;

    public Curse()
    {
        Name = "诅咒";
        Description = "选定一个目标，如果<color=purple>两回合</color>内被选定的目标死亡，抽一张卡牌";
        Cost = 0;
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
        var list = GetUnitList()
            .Select(u=>u.Position);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        _map[target].Units.First()
            .AddBuff(new CurseBuff() { User = user, Time = 2});
    }
}
