using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class SnowStorm : Card
{
    public override CardType Type => CardType.Attack;
    public override CardRarity Rarity => CardRarity.Privilege;
    public SnowStorm()//法师专属
    {
        Name = "暴风雪";
        Description = "对周围角色造成五次<color=red>60%</color>力量值的伤害，并冻结一回合，使其无法行动";
        Cost = 4;
    }

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(2, 2),
        Flags = new bool[5, 5]
        {
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,false,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true }
        }
    };

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return AoeArea.GetPointList(user.Position)
            .Where(p => UniversalFilter(p));
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var list = AoeArea.GetPointList(user.Position);
        targetData.ViewTiles = list;
        targetData.AvaliableTile = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        foreach(var u in GetAffecrTarget(user, target)
            .Where(p=>UniversalFilter(p, true))
            .Select(p=>_map[p].Units.First()))
        {
            for(int i = 0; i < 5; ++i)
            {
                (u as IHurtable).Hurt(user.UnitData.Attack * 0.5f, HurtType.AP | HurtType.FromUnit, user);
            }
            u.AddBuff(new Freeze());
        }
    }
}
