using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// �粼������
/// ��һ����ɫ�������50%����ֵ���˺�������ʱ�����������ĵ��ˣ�
/// �ƶ���������ǰ���й�����������ͣ���ڽ�ɫ��Χ��5x5�ĸ��������)��
/// </summary>
public class Goblingunner : Unit
{
   
    public Goblingunner(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "���ֲ�ǹ��",//���ֲ�ǹ��
        Blood = 100,//��ʼѪ��Ϊ���Ѫ��
        Attack = 20,//������
        Defence = 4,//������
        Speed = 4,//�ȹ�Ȩ��
        ActionPoint = 15,//�ƶ��ͼ��ܵ�ʹ�û����ļ��ܵ�
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
        attackPlayer(player);
        //����
        retreat(player.Position);
    }

    /// <summary>
    /// ������Ҿ���粼�����ֵľ��룬ѡ����ʵĹ�������
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
    /// ��һ����ɫ���3��50%������ֵ�˺�
    /// </summary>
    /// <param name="player">Ҫ���������</param>
    public void attackPlayer(Player player)
    {
        //�ƶ�
        MoveclosePlayerPos(player.Position);
        //����
        for (int i = 0; i < 3; i++)
        {
            (player as IHurtable).Hurt((int)(this.UnitData.Attack * 0.5), HurtType.FromUnit, this);
        }
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
                    if (pos.x == ps.x && pos.y == ps.y)
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
                    if (pos.x == ps.x && pos.y == ps.y)
                    {
                        flag = true;
                        break;
                    }
                }
            }
        }
        Move(pos);
    }

    /*
    /// <summary>
    /// ����ʣ��Ѫ�����������ۺ�ѡ����ʺ�ɱ���Ķ���
    /// </summary>
    /// <param name="players"></param>
    /// <returns></returns>
    public int getAttackPlayerNum(List<Player> players)
    {
        int num = -1;//��¼ѡ�񹥻�����ҵ�index
        int i = 0;
        int nearDeath = int.MaxValue;//���ֵΪ���ֵ

        foreach (Player p in players)
        {
            //��ǰѪ�� ��ȥ���ι������������η�������ӽ�0����Ϊ����
            int remainBlood = p.UnitData.Blood - this.UnitData.Attack * 3 * 50 % + p.UnitData.Defence * 3;
            
            if (Math.Abs(remainBlood) < nearDeath && p.ActionStatus == ActionStatus.Running)
            {
                nearDeath = Math.Abs(remainBlood);
                num = i;
            }
            i++;
        }
        return num;
    }
    /// <summary>
    ///  ����3�ι���
    /// </summary>
    /// <param name="player"></param>
    public void attackPlayer(Player player)
    {
        for(int i = 0; i < 3; i++)
        {
            (player as IHurtable).Hurt((int)(this.UnitData.Attack * 0.5), HurtType.FromUnit, this);
        }
    }
    */
}
