using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonster : Unit
{
    public BaseMonster(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "С��1��",
        Blood = 30,
        Attack = 10,//������
        Defence = 4,//������
        Speed = 2,//�ȹ�Ȩ��
        ActionPoint = int.MaxValue,
    }, pos)
    {
    }

    protected override void Decide()
    {

    }

}
