using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class CurseBuff : RoundBuff
{
    public Unit User;
    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "咒怨";
        res.Description = $"如果<color=purple>{Time}回合</color>内死亡，则<color=blue>{User.UnitData.Name}</color>抽一张卡牌";
        return res;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.UnitDied += Unit_UnitDied;
    }

    private void Unit_UnitDied()
    {
        User.Scheduler.DrawCard();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.UnitDied -= Unit_UnitDied;
    }
}
