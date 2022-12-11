using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalHeal : Card
{
    public override CardType Type => CardType.Heal;

    public NormalHeal()
    {
        Name = "Normal Heal";
        Description = "Heal all friendly unit";
        Cost = 1;
    }
    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return GetUnitList()
            .Where(u => u.Camp == user.Camp)
            .Select(u => u.Position);
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        var targetData = new TargetData();
        targetData.AvaliableTile = GetUnitList()
            .Where(u => u.Camp == user.Camp)
            .Select(u=>u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach(var u in GetAffecrTarget(user, target)
            .Select(p => _map[p].Units.First<ICurable>()))
        {
            u.Cure(user.UnitData.Heal, user);
        }
    }
}
