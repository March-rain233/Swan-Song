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
    public Skeletalmage(Vector2Int pos) : base(new UnitData()
    {
        Name = "Skeletalmage",//���÷�ʦ
        BloodMax = 100,
        Blood = 100,
        Attack = 20,
        Defence = 4,
        Speed = 4,
        //�ƶ��ͼ��ܵ�ʹ�û����ļ��ܵ�,���������ж���Լ������Ϊ���ֵ
        ActionPointMax = int.MaxValue,
        ActionPoint = int.MaxValue,
    }
, pos)
    {
    }
    /// <summary>
    /// �ж�
    /// </summary>
    protected override void Decide()
    {
        //�õ�Ҫ�����Ķ���
        Player player = getAttackPlayer();
        //��������
        attackPlayer(player);
    }

    /// <summary>
    /// ������Ҿ������÷�ʦ�ľ��룬ѡ����ʵĹ�������
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //�����Ҷ���
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
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
    public void attackPlayer(Player player)
    {
        //�����Ҷ���
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();

        foreach (Player p in players)
        {
            if (p.Position.x <= player.Position.x + 1 && p.Position.x >= player.Position.x - 1
             && p.Position.y <= player.Position.y + 1 && p.Position.y >= player.Position.y - 1
                )
            {
                //�����˺�
                (p as IHurtable).Hurt((int)(this.UnitData.Attack * 0.8), HurtType.Ranged, this);
            }
        }
    }

}
