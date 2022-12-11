using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class AngleBless : Card
{
    public override CardType Type => CardType.Heal;

    public AngleBless()//天使庇佑_牧师专属
    {
        Name = "天使庇佑";
        Description = "使一个角色在两回合内血量不能降至1一点以下，同时这两回合额外抽一张卡";
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
        targetData.AvaliableTile = GetUnitList()
            .Where(u => u.Camp == user.Camp)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        int times = 2;
        var tar = (_map[target.x, target.y].Units.First() as ICurable);
        GameManager.Instance.GetState<BattleState>()
            .TurnBeginning += (_) =>
            {
                times -= 1;
                if ((tar as Unit).UnitData.Blood < 1)
                    (tar as Unit).UnitData.Blood = 1;
            };
        user.Scheduler.DrawCard();
    }
}
