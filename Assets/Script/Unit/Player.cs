using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class Player : Unit
{
    public Player(UnitData data, Vector2Int pos) : base(data, pos)
    {
        MoveDistance = 4;
    }

    protected override void Decide()
    {
        
    }

    public void EndDecide()
    {
        EndTurn();
    }
}