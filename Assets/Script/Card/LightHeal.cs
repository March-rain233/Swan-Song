﻿using System;
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
        Description = "回复一个角色（200+120%虔诚值）点生命，若此次治疗使其血量达到最大血量的80%，将血量回复至满";
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
        targetData.AvaliableTile = GameManager.Instance.GetState<BattleState>().UnitList
            .Where(u => u.Camp == user.Camp && u.ActionStatus != ActionStatus.Dead)
            .Select(u => u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        Percent = 1.2f;
        (_map[target.x, target.y].Units.First() as ICurable)
            .Cure(user.UnitData.Heal*Percent+200, user);
        var tar = (_map[target.x, target.y].Units.First() as ICurable);
        var tbmax = (tar as Unit).UnitData.BloodMax;
        if((tar as Unit).UnitData.Blood >= (tar as Unit).UnitData.BloodMax * 0.8)
        {
            tar.Cure(user.UnitData.Heal, user);
        }
    }
}
