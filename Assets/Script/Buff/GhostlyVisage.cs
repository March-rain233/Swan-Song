using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class GhostlyVisage : Buff
{
    public override BuffData GetBuffData()
    {
        var data = base.GetBuffData();
        data.Name = "鬼颜";
        data.Description = $"力量值增加<color=red>{Level * 5}%</color>";
        return data;
    }
    protected override void OnDisable()
    {
        Unit.UnitData.AttackWrapper.Rate /= 1 + 0.05f * Level;
    }

    protected override void OnEnable()
    {
        Unit.UnitData.AttackWrapper.Rate *=  1 + 0.05f * Level;
    }
    public override bool CheckReplace(Buff buff)
    {
        if(buff is GhostlyVisage)
        {
            Unit.UnitData.AttackWrapper.Rate /= 1 + 0.05f * Level;
            Level += 1;
            Unit.UnitData.AttackWrapper.Rate *= 1 + 0.05f * Level;
            Unit.NotifyBuffChange();
        }
        return false;
    }
}
