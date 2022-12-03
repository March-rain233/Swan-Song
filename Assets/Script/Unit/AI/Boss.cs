using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Unit
{
    public Boss(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "BOSS 1ºÅ",
        Blood = 120,
    }, pos)
    {
    }

    protected override void Decide()
    {

    }

    public void EndDecide()
    {
        EndTurn();
    }
}
