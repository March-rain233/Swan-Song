using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// ��Ѫ����
/// ��һ����ɫ���80%����ֵ���˺������ظ���������˺������֮һ��Ѫ����
/// ���ȹ���������͵Ľ�ɫ���ƶ�����ɫ��ǰ���й�����������ͣ���ڽ�ɫ��Χ(5x5�ĸ��������)��
/// </summary>
public class BloodSuckFloater :Unit
{
    public BloodSuckFloater(Vector2Int pos) : base(new UnitData(
        new UnitModel()
        {
            DefaultName = "��Ѫ����",
            Blood = 80,
            Attack = 10,
            Defence = 4,
            Speed = 2
        }), pos)
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
        attackPlayer(player);
        //����
        retreat(player.Position);
    }

    /// <summary>
    /// ������Ҿ�����Ѫ���õľ��룬ѡ����ʵĹ�������
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
    /// �ƶ���Ҫ������Ҹ���
    /// ��һ����ɫ���80������ֵ�˺�
    /// �ָ���������˺������֮һ��Ѫ��
    /// </summary>
    /// <param name="player">Ҫ���������</param>
    public void attackPlayer(Player player)
    {
        //�ƶ�
        MoveclosePlayerPos(player.Position);
        //����
        (player as IHurtable).Hurt((int)(this.UnitData.Attack*0.8), HurtType.FromUnit, this);
        this.UnitData.Blood += (int)(this.UnitData.Attack * 0.8 / 0.5);
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
