using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class LowerHealingPotion : Card
{
    public override CardType Type => CardType.Heal;

    public LowerHealingPotion()//低级治疗药水
    {
        Name= "低级治疗药水";
        Description="恢复<color=green>50</color>血量";
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
        targetData.AvaliableTile = GetUnitList()
            .Where(u => u.Camp == user.Camp)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        (_map[target.x, target.y].Units.First() as ICurable)
            .Cure(50, user);
    }

}
