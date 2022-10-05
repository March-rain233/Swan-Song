using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 白痴单位
/// </summary>
/// <remarks>
/// 木桩
/// </remarks>
public class Idiot : Unit
{
    public Idiot() : base(new UnitData() { 
        BloodMax = int.MaxValue, 
        Blood = int.MaxValue
    }, new())
    {
    }

    protected override void Decide()
    {
        EndTurn();
    }
}