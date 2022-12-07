using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class Hymn : Card
{
    public override CardType Type => CardType.Heal;

    public Hymn()//赞美诗_牧师专属
    {
        Name = "赞美诗";
        Description = "回复所有友方角色一点体力和80%虔诚值点生命";
        Cost = 2;
    }

    protected internal override IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target)
    {
        return GameManager.Instance.GetState<BattleState>().UnitList
            .Where(u => u.Camp == user.Camp)
            .Select(u => u.Position);
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
        if (TileUtility.TryGetTile(target, out var tile) && tile.Units.Count > 0)
        {
            var tar = (_map[target.x, target.y].Units.First() as ICurable);
            (tar as Unit).UnitData.ActionPoint++;
            var healing = user.UnitData.Heal;
            (tile.Units.First() as ICurable).Cure(healing * 0.8f, user);
        }
        else
        {
            Debug.LogWarning("There is no target, please check it.");
        }
    }
}
