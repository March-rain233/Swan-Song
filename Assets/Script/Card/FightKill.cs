using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class FightKill : Card
{
    public override CardType Type => CardType.Attack;

    public FightKill()//搏杀
    {
        Name = "搏杀";
        Description = "移动至目标身前一格，对其造成<color=red>200%</color>力量值的伤害，" +
            "并立刻受到目标<color=red>50%</color>力量值的伤害";
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
        TargetData targetData = new TargetData();
        var list = GameManager.Instance.GetState<BattleState>()
            .UnitList.Where(u=>u.Camp != user.Camp)
            .Select(u=>u.Position)
            .Where(p=>CheckPlaceable((user.Position - p).ToDirection().ToVector2Int() + p, user));
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var tar = _map[target].Units.First();
        var tp = tar.Position;
        Vector2Int dir = (user.Position - tp).ToDirection().ToVector2Int();
        user.Position = tar.Position + dir;
        (tar as IHurtable).Hurt(user.UnitData.Attack * 2, HurtType.FromUnit | HurtType.AD | HurtType.Melee, user);
        (user as IHurtable).Hurt(tar.UnitData.Attack * 0.5f, HurtType.FromUnit | HurtType.AD | HurtType.Melee, tar);
    }
}
