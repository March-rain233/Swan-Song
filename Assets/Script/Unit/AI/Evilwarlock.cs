//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Media;
//using System.Text;
//using UnityEngine;
///// <summary>
///// а����ʿ
///// ͬʱ��������ɫ���100%����ֵ���˺���ÿ���غ϶��⹥��һ��
///// ���⹥����ʹ��ɫ�������غ�����״̬����״̬�ܵ��������˺�����50%
///// Զ�̹������������ƶ�
///// </summary>
//public class EvilWarlock: Unit
//{
//    /// <summary>
//    /// ��һ�غ��˺��Ľ�ɫ
//    /// </summary>
//    public Player hurtedPlayer
//    {
//        get;
//        private set;
//    }
//    public EvilWarlock(Vector2Int pos) : base(new UnitModel()
//    {
//        DefaultName = "а����ʿ",//��Ӣ��
//        Blood = 200,//��ʼѪ��Ϊ���Ѫ��
//        Attack = 25,//������
//        Defence = 10,//������
//        Speed = 2,//�ȹ�Ȩ��
//    }, pos)
//    {
//    }

//    /// <summary>
//    /// �ж�
//    /// </summary>
//    protected override void Decide()
//    {
//        //�����Ҷ���
//        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
//        //�õ�Ҫ�����Ķ���
//        Player p = getAttackPlayer(players);
        
//        //��������
//        attackPlayer(p);
        
//        //����
//        retreat(p.Position);
        
//    }
//    public List<Player> getAttackPlayer()
//    {
//        //�����Ҷ���
//        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();

//        List<Player> attackPlayers = new List<Player>();

//        foreach (Player p in players)
//        {
//            if (p.Position.x >= this.Position.x - 2 && p.Position.x <= this.Position.x + 2
//                && p.Position.y >= this.Position.y - 2 && p.Position.y <= this.Position.y + 2)
//            {
//                attackPlayers.Add(p);
//            }

//        }
//        return attackPlayers;
//    }

//    /// <summary>
//    /// �ƶ������빥����ҽ�Զ�ĵط�
//    /// 
//    /// </summary>
//    /// <param name="player">Ҫ���������</param>
//    public void attackPlayer(Player p)
//    {
//        //�ƶ�
//        MoveclosePlayerPos(p.Position);
//        //�õ���ǰ�غ���
//        int roundNumber = GameManager.Instance.GetState<BattleState>().RoundNumber;
//        foreach (Player p in player)
//        {
//            if(roundNumber <= 3)

//            //100%�˺�
//            (p as IHurtable).Hurt((int)(this.UnitData.Attack * 1), HurtType.FromUnit, this);



//            else//�غ������ڵ���3��
//            {
//                //150%�˺�
//                (p as IHurtable).Hurt((int)this.UnitData.Attack * 1.5), HurtType.FromUnit, this);


//                //-50%����

//                (p as ICurable).Cure((int)this.UnitData.Heal * 0.5)��this);

//            }
//        }
            
  

//    }

//    /// <summary>
//    /// ѡ�����
//    /// </summary>
//    /// <param name="playerPos">���λ��</param>
//    /// <returns></returns>
//    public void MoveclosePlayerPos(Vector2Int playerPos)
//    {
//        //��ȡ�����ƶ���λ��
//        List<Vector2Int> moveablePos = GetMoveArea().ToList();
//        Vector2Int pos = playerPos;
//        bool flag = false;//�Ƿ��ҵ��ɿ�����λ��
        
//        //��Ҹ����а˸�λ�ã��ҵ�һ���ɽ����λ��
        
//        pos = new Vector2Int(playerPos.x + 2, playerPos.y + 2);

//        //�ƶ���������ҽ�Զ�ĵط�
//        Move(pos);
//    }

    
//}