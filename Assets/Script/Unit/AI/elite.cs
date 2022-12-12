using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elite : Unit
{

    public elite(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "精英1号",
        Blood = 80,//初始血量
        Attack = 10,//攻击力
        Defence = 4,//防御力
        Speed = 2,//先攻权重
        ActionPoint = int.MaxValue,
    }
, pos)
    {
        
    }
    //回合制
    protected override void Decide()
    {

    }

}
