using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Freeze : RoundBuff
{
    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "冻结";
        res.Description = "无法行动";
        return res;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.TurnBeginning += Unit_TurnBeginning;
    }

    private void Unit_TurnBeginning()
    {
        Unit.ActionStatus = ActionStatus.Rest;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.TurnBeginning -= Unit_TurnBeginning;
    }
}
