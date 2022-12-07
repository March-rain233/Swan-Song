using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class ForceStorageMatrix : Card
{
    public override CardType Type => CardType.Attack;

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(2, 2),
        Flags = new bool[4, 4]
        {
            { true, true, true, true },
            { true, true, true, true },
            { true, true, true, true },
            { true, true, true, true }
        }
    };

    public ForceStorageMatrix()//蓄力法阵
    {
        Name = "蓄力法阵";
        Description = "两回合后对范围内所有敌人造成<color=red>200%</color>力量值的伤害";
        Cost = 2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AoeArea.GetPointList(target)
            .Where(p=>UniversalFilter(p));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = _map.Select(p=>p.pos);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var sta = GameManager.Instance.GetState<BattleState>();
        int times = 2;
        int attack = user.UnitData.Attack;
        Action<int> callback = null;
        callback = (_) =>
        {
            times -= 1;
            if (times == 0)
            {
                foreach (var u in GetAffecrTarget(user, target)
                    .Where(p => EnemyFilter(p, user.Camp))
                    .Select(p => _map[p].Units.First() as IHurtable))
                {
                    u.Hurt(attack * 2, HurtType.AP | HurtType.Ranged | HurtType.FromUnit, user);
                }
                sta.TurnBeginning -= callback;
            }
        };
        sta.TurnBeginning += callback;
    }
}
