using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Embrittlement : Card
{
    public override CardType Type => CardType.Other;

    public Embrittlement()
    {
        Name = "脆化";
        Description = "选定一个目标，使其防御力减少50%，持续三回合，如果是boss，则只减少10%";
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
        var list = GetUnitList()
            .Where(u => u.Camp != user.Camp)
            .Select(u => u.Position);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var tar = _map[target].Units.First();
        if(tar is Boss)
        {
            tar.AddBuff(new Fragile() { Percent = 0.1f, Time = 3 });
        }
        else
        {
            tar.AddBuff(new Fragile() { Percent = 0.5f, Time = 3 });
        }
    }
}
