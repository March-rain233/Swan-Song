using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 白痴单位
/// </summary>
/// <remarks>
/// 木桩
/// </remarks>
public class Idiot : Unit
{
    public Idiot(Vector2Int pos) : base(new UnitModel()
    {
        Blood = int.MaxValue / 2,
    }, pos)
    {
    }

    protected override void Decide()
    {
        EndTurn();
    }
}