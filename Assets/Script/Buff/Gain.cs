using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Gain: Buff
{
    public override BuffData GetBuffData()
    {
        var data = base.GetBuffData();
        data.Name = "狂怒";
        data.Description = $"力量值增加<color=red>{Level * 10}%</color>";
        return data;
    }
    protected override void OnDisable()
    {
        Unit.UnitData.AttackWrapper.Rate /= 1 + 0.1f * Level;
    }

    protected override void OnEnable()
    {
        Unit.UnitData.AttackWrapper.Rate *= 1 + 0.1f * Level;
    }
    public override bool CheckReplace(Buff buff)
    {
        if (buff is Gain)
        {
            Level = buff.Level + 1;
            return true;
        }
        return false;
    }
}
    


