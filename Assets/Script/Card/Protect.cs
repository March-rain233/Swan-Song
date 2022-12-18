using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Protect : Card
{
    public override CardType Type => CardType.Heal;

    public Protect()//庇护
    {
        Name = "庇护";
        Description = "创建法阵，使其中的角色每回合回复<color=green>（50+50%虔诚值）</color>点生命，两回合后消失";
        Cost = 2;
    }

    public AreaHelper AoeArea = new AreaHelper()
    {
        Center = new Vector2Int(1, 1),
        Flags = new bool[3, 3]
        {
            {true,true,true },
            {true,true,true },
            {true,true,true }
        }
    };

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
        foreach(var t in GetAffecrTarget(user, target)
            .Select(p=>_map[p]))
        {
            t.AddStatus(new HealMatrixStatus() { Heal = 50 + user.UnitData.Heal * 0.5f, Times = 2 });
        }
    }
}
