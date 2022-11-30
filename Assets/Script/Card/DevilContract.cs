using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class DevilContract : Card
{
    public override CardType Type => CardType.Other;

    public DevilContract()
    {
        Name = "恶魔契约";
        Description = "五回合内受到伤害减少50%，造成的伤害增加20%，每回合额外回复1点体力，五回合后立刻死亡";
        Cost = 4;
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
            .Where(u => u.Camp == user.Camp && u.ActionStatus != ActionStatus.Dead && u.Position == user.Position)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        int times = 5;
        GameManager.Instance.GetState<BattleState>()
            .TurnBeginning += (_) =>
            {
                if (times > 0)
                {
                    user.UnitData.Defence = user.UnitData.Defence * 2;
                    user.UnitData.Attack = user.UnitData.Attack * 12 / 10;
                    user.UnitData.ActionPoint++;
                }
                if (times == 0)
                {
                    (user as IHurtable).Hurt(0, HurtType.Death | HurtType.FromUnit, user);
                }
                times -= 1;
            };
    }
}
