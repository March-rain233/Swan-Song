using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Fragile : RoundBuff
{
    public float Percent;
    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "脆弱";
        res.Description = $"防御力减少{Percent * 100}%";
        return res;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.UnitData.DefenceWrapper.Rate *= (1 - Percent);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.UnitData.DefenceWrapper.Rate /= (1 - Percent);
    }
}
