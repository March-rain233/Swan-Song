using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elite : Unit
{

    public elite(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "��Ӣ1��",
        Blood = 80,//��ʼѪ��
        Attack = 10,//������
        Defence = 4,//������
        Speed = 2,//�ȹ�Ȩ��
        ActionPoint = int.MaxValue,
    }
, pos)
    {
        
    }
    //�غ���
    protected override void Decide()
    {

    }

}
