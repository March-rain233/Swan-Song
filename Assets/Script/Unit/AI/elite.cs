using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elite : Unit
{

    public List<Player> PlayerList
    {
        get;
        set;
    } = new();
    public elite(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "��Ӣ1��",
        Blood = 80,//��ʼѪ��Ϊ���Ѫ��
        Attack = 10,//������
        Defence = 4,//������
        Speed = 3,//�ȹ�Ȩ��
        ActionPoint = 20,//��ɫ���ƶ��ͼ��ܵ�ʹ�û����ļ��ܵ�
    }, pos)
    {
        Console.WriteLine("��Ӣ������������ս����");
    }
    //Vector2Int _position = Vector2Int.zero;//Ĭ��0

    //public Vector2Int positionMove
    //{
    //    get => _position;
    //    set
    //    {
    //        var manager = GameToolKit.ServiceFactory.Instance.GetService<GameManager>()
    //            .GetState() as BattleState;
    //        manager.Map[_position.x+1, _position.y+1].Exit(this);
    //        _position = value;
    //        manager.Map[_position.x+1, _position.y+1].Enter(this);
    //    }
    //}
    //�غ���
    protected override void Decide()
    {
        //�ƶ���Ӣ ���ݺ���ҵľ���ȷ���ƶ�λ�ã�
        //Move(this.Position + this.positionMove);//��ǰλ�� + �ƶ�λ�� = �ƶ����λ��

        //�ͷſ���
        List<Card> cards = this.UnitData.Deck;
        //����һ���Ĳ���ѡ��Ҫ�ͷſ��ƣ��õ����ƺ���rcn
        int rcn = getNumCardBest(cards);
        //foreach (Card c in cards)
        //{
        //    c.Release(this, this.Position);
        //}    
    }
    //�����غ�
    public void EndDecide()
    {
        EndTurn();
    }
    //�ۺ��ж��㡢��ǰ���ƵĹ������������������ۺ�ѡ����ʺ��ͷŵĿ���
    public int getNumCardBest(List<Card>  cards)
    {
        int num = -1;
        //......
        return num;
    }
}
