using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
/// <summary>
///��һ����ɫ������˿�����20%����ֵ���˺���ʹ�䱾�غ��޷��ƶ���
///���һ����ɫ���ֶ�ս��ͬһ˿���������Σ��γ�����������
///���ֶ�սʣ��ʱ���޷��ƶ�(�����༼�ܳ���)�����ȹ�������������ߵĽ�ɫ��Զ�̹��������ƶ���
/// </summary>
public class SilkSpider : Unit
{

    public SilkSpider(Vector2Int pos) : base(new UnitData()
    {
        Name = "SilkSpider",//˿��
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
    ///     ��˿�빥���������
    /// </summary>
    public Dictionary<Unit,int> attackedPlayers = new Dictionary<Unit, int>();
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
    /// ���ȹ�������������ߵĽ�ɫ
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //�����Ҷ���
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = -1;
        int i = 0;
        double maxAction = int.MinValue;//���ֵΪ��Сֵ

        foreach (Player p in players)
        {
            if (p.UnitData.ActionPoint > maxAction && p.ActionStatus == ActionStatus.Running)
            {
                maxAction = p.UnitData.ActionPoint;
                num = i;
            }
            i++;
        }
        return players[num];
    }
    /// <summary>
    /// ��һ����ɫ���20%������ֵ�˺�
    /// ʹ�䱾�غ��޷��ƶ�
    /// ���һ����ɫ���ֶ�ս��ͬһ˿���������Σ��γ���������
    /// </summary>
    /// <param name="player">Ҫ���������</param>
    public void attackPlayer(Player player)
    {
        //����
        (player as IHurtable).Hurt(this.UnitData.Attack * 0.2f, HurtType.Ranged, this);
        Restrain restrain = new Restrain();
        restrain.Times = 1;
        player.AddBuff(restrain);

        bool attacked = false;//�Ƿ񱻹�����
        foreach(var p in attackedPlayers.Keys)
        {
            if(p == player)
            {
                attacked = true;
                attackedPlayers[p]++;
                if(attackedPlayers[p] == 3)
                {
                    restrain.Times = int.MaxValue;//�����ƶ���buffʩ��ʱ�����޳��������ֶ����ܶ�
                }
                break;
            }
        }
        if (!attacked)
        {
            attackedPlayers.Add(player,1);
        }

    }


}

