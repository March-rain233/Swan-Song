using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// �綾֩��: 
/// ��һ����ɫ���50%����ֵ���˺���
/// ���ȹ���Ѫ�����ٵĵ��ˣ�
/// ���������غϵ��ж�Ч����
/// ÿ�غ����10%����ֵ���˺������˺����ӷ�����
/// ������ԭ��3x3�ķ�Χ���ɶ�Һ���Σ�
/// �������ĵ���ʩ�������ж�Ч������Һ���δ������غϣ�
/// �ƶ���������ǰ���й���,������ͣ���ڽ�ɫ��Χ(5x5�ĸ��������)��
/// </summary>
public class FungalSpider : Unit
{
    public FungalSpider(Vector2Int pos) : base(new UnitData()
    {
        Name = "FungalSpider",//�綾֩��
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
    /// ���ȹ���Ѫ�����ٵĵ���
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //�����Ҷ���
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = -1,i = 0;//��¼Ѫ�����ٵ��������
        int lessBlood = int.MaxValue;//���ֵΪ���ֵ

        foreach (Player p in players)
        {
            if(p.UnitData.Blood < lessBlood)
            {
                lessBlood = p.UnitData.Blood;
                num = i;
            }
            i++;
        }
        return players[num];
    }
    /// <summary>
    /// �ƶ���Ҫ������Ҹ���
    /// ��һ����ɫ���50%������ֵ�˺�
    /// �������غϵ��ж�Ч����
    /// ÿ�غ����10%����ֵ���˺������˺����ӷ���
    /// </summary>
    /// <param name="player">Ҫ���������</param>
    public void attackPlayer(Player player)
    {
        //�ƶ�
        MoveclosePlayerPos(player.Position);
        //����
        (player as IHurtable).Hurt(this.UnitData.Attack * 0.5f, HurtType.Melee, this);
        //�ж�buff
        Poison poison = new Poison();
        poison.Times = 3;
        poison.Damage = this.UnitData.Attack * 0.1f;
        player.AddBuff(poison);
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

    /// <summary>
    /// ������ԭ��3x3�ķ�Χ���ɶ�Һ����
    /// �������ĵ���ʩ�������ж�Ч������Һ���δ������غ�
    /// </summary>
    protected override void OnDied()
    {
        //�����Ҷ���
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        Poison poison = new Poison();
        poison.Times = 3;
        poison.Damage = this.UnitData.Attack * 0.1f;
        foreach (Player p in players)
        {
            if (p.Position.x <= this.Position.x + 1 && p.Position.x >= this.Position.x - 1
             && p.Position.y <= this.Position.y + 1 && p.Position.y >= this.Position.y - 1
                )
            {
                //����ж�buff
                p.AddBuff(poison);
            }
        }
    }
    
}
