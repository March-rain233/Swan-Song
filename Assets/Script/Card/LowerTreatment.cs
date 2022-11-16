using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class LowerTreatment : Card
{
    public override CardType Type => CardType.Heal;

    public LowerTreatment()//低级治疗术
    {
        Name="LowerTreatment";
        Description="Heal 40 health and 40% Religious values";
        Cost=1;
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
            .Where(u => u.Camp == user.Camp && u.ActionStatus != ActionStatus.Dead)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var healing = user.UnitData.Heal;
        Percent = 0.4f;
        (_map[target.x, target.y].Units.First() as ICurable)
            .Cure(60+Percent*healing, user);
    }

}
