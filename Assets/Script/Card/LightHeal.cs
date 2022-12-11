using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LightHeal : Card
{
    public override CardType Type => CardType.Heal;

    public LightHeal()//束光愈_牧师专属
    {
        Name = "束光愈";
        Description = "回复一个目标<color=green>（200+120%虔诚值）</color>点生命值，" +
            "若此次治疗使其血量达到最大血量的80%，将血量回复至满";
        Cost = 3;
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
        var list = GetUnitList()
            .Where(u => u.Camp == user.Camp)
            .Select(u => u.Position);
        targetData.AvaliableTile = list;
        targetData.ViewTiles = list;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var tar = (_map[target.x, target.y].Units.First() as ICurable);
        tar.Cure(user.UnitData.Heal * 1.2f + 200, user);
        if((tar as Unit).UnitData.Blood >= (tar as Unit).UnitData.BloodMax * 0.8)
        {
            tar.Cure(user.UnitData.BloodMax, user);
        }
    }
}
