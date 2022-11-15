using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// ӰϮ��
/// ��һ����ɫ���100%����ֵ���˺��������������ͬһ��ɫ��
/// �ӵڶ��ο�ʼ��ÿ�����150%����ֵ���˺�
/// �������һ�غϴ��˽�ɫA���ڶ��غϻ���Ľ�ɫA�����˺����������м�ת�ƹ�Ŀ���Ҫ���¼���)��
/// ���ȹ���Ѫ���ٷֱ���͵ĵ��ˣ��ƶ�����ɫ��ǰ���й����������󷵻�ԭλ�á�
/// </summary>
public class ShaAttkMonster : Unit
{
    /// <summary>
    /// ��һ�غ��˺��Ľ�ɫ
    /// </summary>
    public Player hurtedPlayer
    {
        get;
        private set;
    }
    public ShaAttkMonster(Vector2Int pos) : base(new UnitData()
    {
        Name = "ShaAttkMonster",//ʷ��ķ
        BloodMax = 80,//���Ѫ��
        Blood = 80,//��ʼѪ��Ϊ���Ѫ��
        Attack = 10,//������
        Defence = 4,//������
        Speed = 2,//�ȹ�Ȩ��
    }
, pos)
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
    /// ����Ѫ����ѡ����ʵĹ�������
    /// </summary>
    /// <param name="players">�������</param>
    /// <returns></returns>
    public Player getAttackPlayer(List<Player> players)
    {
        int num = -1;//��¼Ѫ�������ٵ���ҵĺ���
        int i = 0;
        double minbloodPercent = int.MaxValue;//���ֵΪ���ֵ

        foreach (Player p in players)
        {
            double bloodPercent = (double)p.UnitData.Blood / p.UnitData.BloodMax;
            if (bloodPercent < minbloodPercent && p.ActionStatus == ActionStatus.Running)
            {
                minbloodPercent = bloodPercent;
                num = i;
            }
            i++;
        }
        return players[num];
    }
    /// <summary>
    /// �ƶ���Ҫ������Ҹ���
    /// ��һ����ɫ���100%����ֵ���˺��������������ͬһ��ɫ��
    /// �ӵڶ��ο�ʼ��ÿ�����150%����ֵ���˺�
    /// </summary>
    /// <param name="player">Ҫ���������</param>
    public void attackPlayer(Player player)
    {
        //�ƶ�
        MoveclosePlayerPos(player.Position);
        //�õ���ǰ�غ���
        int roundNumber = GameManager.Instance.GetState<BattleState>().RoundNumber;
        //���ǵ�һ�غϣ�ӰϮ���Ѿ����������жϵ�ǰ�غϹ�������Ƿ�����һ�ֹ������Ķ���
        if (roundNumber != 1 && this.hurtedPlayer == player)
        {
            //150%�˺�
            (player as IHurtable).Hurt((int)(this.UnitData.Attack*1.5), HurtType.FromUnit, this);
        }
        else//�ǵ�һ�غϻ����ι�������һ��
        {
            //100%�˺�
            (player as IHurtable).Hurt(this.UnitData.Attack, HurtType.FromUnit, this);
        }
        this.hurtedPlayer = player;
       
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
