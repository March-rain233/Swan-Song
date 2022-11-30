using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 根据回合数减少计数的Buff
/// </summary>
public abstract class RoundBuff : Buff
{
    protected override void OnEnable()
    {
        Unit.TurnBeginning += Unit_TurnBeginning;
    }

    private void Unit_TurnBeginning()
    {
        if(Count > 0)
        {
            Count -= 1;
        }
        else if(Count == 0)
        {
            Unit.RemoveBuff(this);
        }
    }

    protected override void OnDisable()
    {
        Unit.TurnBeginning -= Unit_TurnBeginning;
    }
}
