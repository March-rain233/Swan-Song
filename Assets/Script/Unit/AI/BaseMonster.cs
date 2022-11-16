using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonster : Unit
{
    public BaseMonster(Vector2Int pos) : base(new UnitData()
    {
        Name = "Ð¡±ø1ºÅ",
        BloodMax = 30,
        Blood = 30,
        Deck = new(),
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
