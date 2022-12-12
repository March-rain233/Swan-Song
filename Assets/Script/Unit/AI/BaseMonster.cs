using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonster : Unit
{
    public BaseMonster(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "小兵1号",
        Blood = 30,
        Attack = 10,//攻击力
        Defence = 4,//防御力
        Speed = 2,//先攻权重
        ActionPoint = int.MaxValue,
    }, pos)
    {
    }

    protected override void Decide()
    {

    }

}
