using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalHeal : Card
{
    public NormalHeal()
    {
        Name = "Normal Heal";
        Description = "Heal all friendly unit";
        Cost = 1;
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
            .Select(u=>u.Position);
        targetData.ViewTiles = targetData.AvaliableTile;
        return targetData;
    }

    protected internal override void Release(Unit user, Vector2Int target)
    {
        if(TileUtility.TryGetTile(target, out var tile) && tile.Units.Count>0)
        {
            var healing = user.UnitData.Heal;
            (tile.Units.First() as ICurable).Cure(healing, user);
        }
        else
        {
            Debug.LogWarning("There is no target, please check it.");
        }
    }
}
