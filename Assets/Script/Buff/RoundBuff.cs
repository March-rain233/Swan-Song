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
    /// <summary>
    /// 剩余计数
    /// </summary>
    public int Time = -1;
    protected override void OnEnable()
    {
        Unit.TurnBeginning += Unit_TurnBeginning;
    }

    private void Unit_TurnBeginning()
    {
        if(Time > 0)
        {
            Time -= 1;
        }
        if(Time == 0)
        {
            Unit.RemoveBuff(this);
        }
        Unit.NotifyBuffChange();
    }

    protected override void OnDisable()
    {
        Unit.TurnBeginning -= Unit_TurnBeginning;
    }

    public override bool CheckReplace(Buff buff)
    {
        return base.CheckReplace(buff) 
            || (buff.Level == Level && buff is RoundBuff && (buff as RoundBuff).Time > Time);
    }

    public override BuffData GetBuffData()
    {
        var data = base.GetBuffData();
        data.Time = Time;
        return data;
    }
}
