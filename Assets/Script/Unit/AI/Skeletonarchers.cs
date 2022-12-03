using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// ���ù���
/// ��һ���ڵĵ�λ���乭�������100%����ֵ���˺���
/// ������������Ľ�ɫ��ԭ�ؽ���Զ�̹������������ƶ���
/// </summary>
public class Skeletonarchers : Unit
{ 
    public Skeletonarchers(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "���ù���",//���ù���
        Blood = 100,
        Attack = 20,
        Defence = 4,
        Speed = 4,
    }, pos)
    {

    }

    protected override void Decide()
    {
        //�����Ҷ���
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        //�õ�Ҫ�����Ķ���
        Player player = getAttackPlayer(players);
        //��������
        attackPlayer(player);
    }
    /// <summary>
    /// ������Ҿ������ù��ֵľ��룬ѡ����ʵĹ�������
    /// </summary>
    /// <param name="players">�������</param>
    /// <returns></returns>
    public Player getAttackPlayer(List<Player> players)
    {
        int num = -1;//��¼������̵���ҵĺ���
        int i = 0;
        double minDis = int.MaxValue;//���ֵΪ���ֵ

        foreach (Player p in players)
        {
            double dis = Math.Pow(Math.Abs(p.Position.x - this.Position.x), 2.0) + Math.Pow(Math.Abs(p.Position.y - this.Position.y), 2.0);
            if (dis < minDis && p.ActionStatus == ActionStatus.Running)
            {
                minDis = dis;
                num = i;
            }
            i++;
        }
        return players[num];
    }
    /// <summary>
    /// ��һ���ڵĵ�λ���乭�������100%����ֵ���˺�
    /// </summary>
    /// <param name="player"></param>
    public void attackPlayer(Player player)
    {
        //����
        (player as IHurtable).Hurt(this.UnitData.Attack, HurtType.FromUnit, this);
    }
}
