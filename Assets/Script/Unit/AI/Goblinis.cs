using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �粼����ʿ
/// ��һ����ɫ������Χ�˸��ڵĽ�ɫ���100%����ֵ���˺�������ʱ�����������ĵ���,
/// �ƶ���������ǰ���й���,������ͣ���ڽ�ɫ��Χ(5x5�ĸ��������)��
/// </summary>
public class Goblinis : Unit
{
    public Goblinis(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "���ֲ���ʿ",//���ֲ���ʿ
        Blood = 100,//��ʼѪ��Ϊ���Ѫ��
        Attack = 20,//������
        Defence = 4,//������
        Speed = 4,//�ȹ�Ȩ��
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
        attackPlayer(players,player);
        //����
        retreat(player.Position);
    }

    /// <summary>
    /// ������Ҿ���粼����ʿ�ľ��룬ѡ����ʵĹ�������
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
    /// </summary>
    /// <param name="player"></param>
    public void attackPlayer(List<Player> players, Player player)
    {
        //�ƶ�
        MoveclosePlayerPos(player.Position);
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                int index = hasPlayer(players, new Vector2Int(player.Position.x + i, player.Position.y + j));
                if (index != -1)//�����λ���н�ɫ���й���
                {
                    (players[index] as IHurtable).Hurt(this.UnitData.Attack, HurtType.FromUnit, this);
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
    /// <summary>
    /// �������
    /// </summary>
    /// <param name="playerPos">���λ��</param>
    /// <returns></returns>
    public void MoveclosePlayerPos(Vector2Int playerPos)
    {
        //��ȡ�����ƶ���λ��
        List<Vector2Int> moveablePos = GetMoveArea().ToList();
        Vector2Int pos = playerPos;
        bool flag = false;//�Ƿ��ҵ��ɿ�����λ��
        //��Ҹ����а˸�λ�ã��ҵ�һ���ɽ����λ��
        for (int i = -1; i <= 1 && !flag; ++i)
        {
            for (int j = -1; j <= 1 && !flag; ++j)
            {
                pos = new Vector2Int(playerPos.x + i, playerPos.y + j);

                foreach (Vector2Int ps in moveablePos)
                {
                    if (pos == ps)
                    {
                        flag = true;
                        break;
                    }
                }
            }
        }
        //�ƶ�����Ҹ���
        Move(pos);
    }

    /// <summary>
    /// ���˵���Ҹ���5*5������
    /// </summary>
    /// <param name="playerPos">����������ҵ�λ��</param>
    public void retreat(Vector2Int playerPos)
    {
        //��ȡ�����ƶ���λ��
        List<Vector2Int> moveablePos = GetMoveArea().ToList();
        Vector2Int pos = playerPos;
        bool flag = false;//�Ƿ��ҵ��ɿ�����λ��
        //��Ҹ����а˸�λ�ã��ҵ�һ���ɽ����λ��
        for (int i = -2; i <= 2 && !flag; ++i)
        {
            for (int j = -2; j <= 2 && !flag; ++j)
            {
                pos = new Vector2Int(playerPos.x + i, playerPos.y + j);

                foreach (Vector2Int ps in moveablePos)
                {
                    //�жϸ�λ���Ƿ�ɳ���
                    if (pos == ps)
                    {
                        flag = true;
                        break;
                    }
                }
            }
        }
        Move(pos);
    }
}
