using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Stun : RoundBuff
{
    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.TurnBeginning -= Unit_TurnBeginning;
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

    public override BuffData GetBuffData()
    {
        var data = base.GetBuffData();
        data.Name = "晕眩";
        data.Description = "无法行动";
        return data;
    }
}
