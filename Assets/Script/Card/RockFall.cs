﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class RockFall : Card
{
    AreaHelper AreaHelper;
    public override CardType Type => CardType.Attack;

    public RockFall(Unit user)//战士专属
    {
        Name = "岩崩斩";
        Description = "向前方斩出一击，对直线上敌人造成200%力量值的伤害，对血量低于50%的敌人额外造成一次100%力量值的伤害";
        Cost = 3;
    }

    protected internal override TargetData GetAvaliableTarget(Unit user)
    {
        TargetData data = new TargetData();
        data.ViewTiles = new List<Vector2Int>();
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Up));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Down));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Left));
        data.ViewTiles.Union(AreaHelper.GetPointList(user.Position, Direction.Right));
        data.ViewTiles = data.ViewTiles.Where(p =>
            0 <= p.x && p.x < _map.Width
            && 0 <= p.y && p.y < _map.Height
            && _map[p.x, p.y] != null);
        data.AvaliableTile = data.ViewTiles;
        return data;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        var dir = (target - user.Position).ToDirection();
        return AreaHelper.GetPointList(user.Position, dir)
            .Where(p =>
            0 <= p.x && p.x < _map.Width
            && 0 <= p.y && p.y < _map.Height
            && _map[p.x, p.y] != null
            &&_map[p.x, p.y].Units.First().Camp != user.Camp);
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        var list = GetAffecrTarget(user, target)
            .Where(p => _map[p.x, p.y].Units.Count > 0)
            .Select(p => _map[p.x, p.y].Units.First());
        foreach (IHurtable u in list)
        {
            u.Hurt(user.UnitData.Attack * 2, HurtType.AD | HurtType.FromUnit, user);
            if((u as Unit).UnitData.Blood <= (u as Unit).UnitData.BloodMax / 2)
            {
                u.Hurt(user.UnitData.Attack , HurtType.AD | HurtType.FromUnit, user);
            }
        }
    }
}