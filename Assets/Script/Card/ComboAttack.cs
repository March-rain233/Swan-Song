using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class ComboAttack : Card
{
    public override CardType Type => CardType.Attack;
    public float Percent = 0.5f;
    public float AddPercent = 0.2f;

    public AreaHelper AttackArea = new AreaHelper()
    {
        Center = new Vector2Int(0, 0),
        Flags = new bool[2, 1]
        {
            {false },
            {true }
        }
    };

    public ComboAttack()//连击
    {
        Name = "连击";
        Description = $"对敌人造成<color=red>{Percent * 100}%</color>力量值的伤害，" +
            $"本局对战中每使用过一次，就增加<color=red>{AddPercent * 100}</color>力量值的伤害";
        Cost = 1;
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
        var position = user.Position;
        var map = _map;
        var list = AttackArea.GetPointList(position);
        targetData.ViewTiles = list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null
            && map[p.x, p.y].Units.First().Camp != user.Camp);
        targetData.AvaliableTile = targetData.ViewTiles.Where(p => map[p.x, p.y].Units.Count > 0);
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        (_map[target.x, target.y].Units.First() as IHurtable)
            .Hurt(Percent * user.UnitData.Attack, HurtType.AD | HurtType.FromUnit | HurtType.Melee, user);
            
        Percent += AddPercent;
        Description = $"对敌人造成<color=red>{Percent * 100}%</color>力量值的伤害，" +
            $"本局对战中每使用过一次，就增加<color=red>{AddPercent * 100}</color>力量值的伤害";
    }
}
