using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class StoneAttack : Card
{
    public override CardType Type => CardType.Attack;

    public StoneAttack()//投石
    {
        Name="投石";
        Description= "对选中单位造成<color=red>40%</color>力量值的伤害";
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
        TargetData targetData = new TargetData();
        var list = GameManager.Instance.GetState<BattleState>().UnitList
            .Where(u=>u.Camp != user.Camp && u.ActionStatus != ActionStatus.Dead)
            .Select(u=>u.Position);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        (_map[target.x, target.y].Units.First() as IHurtable)
            .Hurt(user.UnitData.Attack * 0.4f, HurtType.FromUnit | HurtType.AD | HurtType.Ranged, user);
    }
}
