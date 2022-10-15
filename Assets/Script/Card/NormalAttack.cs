using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NormalAttack : Card
{
    public AreaHelper AttackArea;

    public NormalAttack()
    {
        Name = "NormalAttack";
        Cost = 1;
        Description = "Attack Enemy";
    }
    protected internal override TargetData GetActionRange(Unit user, TargetData targetData)
    {
        return targetData;
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData targetData = new TargetData();
        var position = user.Position;
        var map = _map;
        var list = AttackArea.GetPointList(position);
        targetData.Tiles = list.Where(p =>
            0 <= p.x && p.x < map.Width
            && 0 <= p.y && p.y < map.Height
            && map[p.x, p.y] != null);
        targetData.Units = targetData.Tiles.Where(p => map[p.x, p.y].Units.Count > 0)
            .Select(p => map[p.x, p.y].Units.First());
        return targetData;
    }

    protected internal override void Release(Unit user, TargetData targetData)
    {
        (targetData.Units.First() as IHurtable)
            .Hurt(user.UnitData.Attack, HurtType.FromUnit, user);
    }
}