using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// ���÷�ʦ
/// ��һ��3x3�����ڵ����ез���ɫ���80%����ֵ���˺���
/// ������������Ľ�ɫ��ԭ�ؽ���Զ�̹������������ƶ���
/// </summary>
public class Skeletalmage : Unit
{
    public Skeletalmage(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "���÷�ʦ",//���÷�ʦ
        Blood = 100,
        Attack = 20,
        Defence = 4,
        Speed = 4,
    }, pos)
    {
    }
    /// <summary>
    /// �ж�
    /// </summary>
    protected override void Decide()
    {
        //�����Ҷ���
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        //�õ�Ҫ�����Ķ���
        Player player = getAttackPlayer(players);
        //��������
        attackPlayer(players, player);
    }

    /// <summary>
    /// ������Ҿ������÷�ʦ�ľ��룬ѡ����ʵĹ�������
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
    ///  ���з�Χ����
    ///  ��һ��3x3�����ڵ����ез���ɫ���80%����ֵ���˺�
    /// </summary>
    /// <param name="player"></param>
    public void attackPlayer(List<Player> players, Player player)
    {
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                int index = hasPlayer(players, new Vector2Int(player.Position.x + i, player.Position.y + j));
                if (index != -1)//�����λ���н�ɫ���й���
                {
                    (players[index] as IHurtable).Hurt((int)(this.UnitData.Attack * 0.8), HurtType.FromUnit, this);
                }
            }
        }
    }
    /// <summary>
    /// �ж��Ƿ�����������λ����
    /// </summary>
    /// <param name="players"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int hasPlayer(List<Player> players, Vector2Int pos)
    {
        int i = 0;
        foreach (Player player in players)
        {
            if (player.Position == pos)
            {
                return i;
            }
            i++;
        }
        return -1;
    }
}
