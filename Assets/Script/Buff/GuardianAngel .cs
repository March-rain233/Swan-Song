using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GuardianAngel : RoundBuff
{
    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "庇护天使";
        res.Description = $"血量无法降至1一点以下，每回合额外抽一张卡";
        return res;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.Scheduler.DrawCard();
        Unit.Preparing += Unit_Preparing;
        Unit.Hurt += Unit_Hurt;
    }

    private void Unit_Hurt(float arg1, HurtType arg2, object arg3)
    {
        if(Unit.UnitData.Blood <= 0)
        {
            Unit.UnitData.Blood = 1;
        }
    }

    private void Unit_Preparing()
    {
        Unit.Scheduler.DrawCard();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.Preparing -= Unit_Preparing;
        Unit.Hurt -= Unit_Hurt;
    }
}
