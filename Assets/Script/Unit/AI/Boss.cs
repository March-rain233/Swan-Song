using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Unit
{
    public Boss(Vector2Int pos) : base(new UnitData()
    {
        Name = "BOSS 1��",
        BloodMax = 120,//���Ѫ��
        Blood = 120,//��ʼѪ��Ϊ���Ѫ��
        Deck = new(),//���е��ƿ�
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
