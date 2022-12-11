using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Hymn : Card
{
    public override CardType Type => CardType.Heal;

    public Hymn()//赞美诗_牧师专属
    {
        Name = "赞美诗";
        Description = "回复所有友方角色<color=yellow>1</color>点体力和<color=green>80%</color>虔诚值点的生命";
        Cost = 2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return GameManager.Instance.GetState<BattleState>().UnitList
            .Where(u => u.Camp == user.Camp)
            .Select(u => u.Position);
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
        foreach(var u in GetAffecrTarget(user, target)
            .Select(p => _map[p].Units.First()))
        {
            u.UnitData.ActionPoint += 1;
            (u as ICurable).Cure(user.UnitData.Heal * 0.8f, user);
        }
    }
}
