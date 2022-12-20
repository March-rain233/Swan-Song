using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class GainBoss: RoundBuff
{

    protected override void OnEnable()
    {
        base.OnEnable();

        Unit.UnitData.AttackWrapper.Rate = 1.1f;
        Unit.UnitData.Blood = (int)(Unit.UnitData.Blood *1.1f);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.UnitData.AttackWrapper.Rate = 1;
        Unit.UnitData.Blood = (int)(Unit.UnitData.Blood * 1);
    }


    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "增益";
        res.Description = "Boss增益";
        return res;
    }

}



