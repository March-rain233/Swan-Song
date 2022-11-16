using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Unit
{
    public Boss(Vector2Int pos) : base(new UnitData()
    {
        Name = "BOSS 1号",
        BloodMax = 120,//最大血量
        Blood = 120,//初始血量为最大血量
        Deck = new(),//持有的牌库
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
