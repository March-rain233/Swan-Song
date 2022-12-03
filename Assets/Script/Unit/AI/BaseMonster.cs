using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonster : Unit
{
    public BaseMonster(Vector2Int pos) : base(new UnitData(
        new UnitModel()
        {
            DefaultName = "Ð¡±ø1ºÅ",
            Blood = 30,
        }), pos)
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
