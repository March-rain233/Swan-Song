using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class CounterPunch : Card
{
    public override CardType Type => CardType.Attack;

    public CounterPunch()
    {
        Name = "Counter Punch";
        Description = "Deal counter damage to enemy";
        Cost = 1;
    }
    protected internal override void Release(Unit user, Vector2Int target)
    {
        Action<float, HurtType, object> callback = null;
        callback = (float arg1, HurtType arg2, object arg3) =>
        {
            if (arg2.HasFlag(HurtType.Melee | HurtType.FromUnit))
            {
                var tar = arg3 as IHurtable;
                tar.Hurt(user.UnitData.Attack, HurtType.FromUnit | HurtType.AD | HurtType.Melee, user);
                user.Hurt -= callback;
            }
        };
        user.Hurt += callback;
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        return new TargetData()
        {
            AvaliableTile = new List<Vector2Int>() { user.Position },
            ViewTiles = new List<Vector2Int>() { user.Position },
        };
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return new List<Vector2Int>() { user.Position };
    }
}
