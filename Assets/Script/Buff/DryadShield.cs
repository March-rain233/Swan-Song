using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DryadShield : Buff
{
    public override BuffData GetBuffData()
    {
        var data = base.GetBuffData();
        data.Name = "林妖庇护";
        data.Description = $"免疫{Level}次中毒或晕眩效果";
        return data;
    }
    protected override void OnDisable()
    {
        Unit.AddBuffHandler -= Unit_AddBuffHandler;
    }

    protected override void OnEnable()
    {
        Unit.AddBuffHandler += Unit_AddBuffHandler;
    }

    private bool Unit_AddBuffHandler(Buff arg)
    {
        if(arg is Stun || arg is Poison)
        {
            Level -= 1;
            if(Level <= 0)
            {
                Unit.RemoveBuff(this);
            }
            return false;
        }
        return true;
    }
}

