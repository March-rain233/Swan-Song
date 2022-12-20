using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Gain: RoundBuff
{

    protected override void OnEnable()
    {
        base.OnEnable();

        Unit.UnitData.AttackWrapper.Rate = 1.1f;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.UnitData.AttackWrapper.Rate = 1;
    }

    
    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "增伤";
        res.Description = "怪物增伤";
        return res;
    }

}
    


