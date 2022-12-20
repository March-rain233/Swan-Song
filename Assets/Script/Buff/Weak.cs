using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Weak : RoundBuff
{
    public float Percent;
    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "虚弱";
        res.Description = $"受到的伤害增加50%, 受到的治疗效果减少50%";
        return res;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.HurtCalculating += Unit_HurtCalculating;
        Unit.CureCalculating += Unit_CureCalculating;
    }

    private void Unit_CureCalculating(CureCalculateEvent obj)
    {
        obj.Rate *= 0.5f;
    }

    private void Unit_HurtCalculating(HurtCalculateEvent obj)
    {
        obj.Rate *= 1.5f;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.HurtCalculating -= Unit_HurtCalculating;
        Unit.CureCalculating -= Unit_CureCalculating;
    }
}
