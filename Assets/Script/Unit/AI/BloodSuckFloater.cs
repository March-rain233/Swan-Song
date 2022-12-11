using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��Ѫ����
/// ��һ����ɫ���80%����ֵ���˺������ָ���������˺������֮һ��Ѫ����
/// ���ȹ���������͵Ľ�ɫ���ƶ�����ɫ��ǰ���й�����������ͣ���ڽ�ɫ��Χ(5x5�ĸ��������)��
/// </summary>
public class BloodSuckFloater :Unit
{
    public BloodSuckFloater(Vector2Int pos) : base(new UnitData()
    {
        Name = "BloodSuckFloater",//��Ѫ����
        BloodMax = 80,//���Ѫ��
        Blood = 80,//��ʼѪ��Ϊ���Ѫ��
        Attack = 10,//������
        Defence = 4,//������
        Speed = 2,//�ȹ�Ȩ��
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
        //����
        retreat(player.Position);
    }

    /// <summary>
    /// ���ȹ���������͵Ľ�ɫ
    /// </summary>
    /// <returns>Ҫ���������</returns>
    public Player getAttackPlayer()
    {
        //�����Ҷ���
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = -1;//��¼������͵���ҵ�����
        int i = 0;
        double minDifence = int.MaxValue;//���ֵΪ���ֵ

        foreach (Player p in players)
        { 
            if (p.UnitData.Defence < minDifence && p.ActionStatus == ActionStatus.Running)
            {
                minDifence = p.UnitData.Defence;
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
