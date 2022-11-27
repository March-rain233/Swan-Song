using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class SelfishWish : Card
{
    public override CardType Type => CardType.Other;

    public SelfishWish()
    {
        Name = "自私的祝福";
        Description = "选定一个友方角色，在本局对战中，此角色每造成一次伤害，回复施法者20点生命";
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
            .Where(u => u.Camp == user.Camp && u.ActionStatus != ActionStatus.Dead)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        int times = 1;
        var tar = (_map[target.x, target.y].Units.First() as IHurtable);
        GameManager.Instance.GetState<BattleState>()
            .TurnBeginning += (_) =>
            {
                times -= 1;
                if (times == 0 && (tar as Unit).ActionStatus == ActionStatus.Running)
                {
                    (_map[target.x, target.y].Units.First() as ICurable)
                    .Cure(20, user);
                }
            };
    }
}
