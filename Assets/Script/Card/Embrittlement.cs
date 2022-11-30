using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Embrittlement : Card
{
    public override CardType Type => CardType.Other;

    public Embrittlement()
    {
        Name = "脆化";
        Description = "选定一个目标，使其防御力减少50%，持续三回合，如果是boss，则只减少10%";
        Cost = 1;
    }

    public AreaHelper AttackArea = new AreaHelper()
    {
        Center = new Vector2Int(2, 2),
        Flags = new bool[5, 5]
        {
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true },
            {true,true,true,true,true }
        }
    };

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        res.Add(target);
        return res;
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var position = user.Position;
        var map = _map;
        var list = AttackArea.GetPointList(position);
        targetData.ViewTiles = list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null);
        targetData.AvaliableTile = targetData.ViewTiles.Where(p => map[p.x, p.y].Units.Count > 0);
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        int times = 3;
        GameManager.Instance.GetState<BattleState>()
            .TurnBeginning += (_) =>
            {
                times -= 1;
                if (times >= 0 && _map[target.x, target.y].Units is not Boss)
                {
                    user.UnitData.Defence = user.UnitData.Defence / 2;
                }
                else if(times >= 0 && _map[target.x, target.y].Units is Boss)
                {
                    user.UnitData.Defence = user.UnitData.Defence / 10 * 9;
                }
            };
    }
}
