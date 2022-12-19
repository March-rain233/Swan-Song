using System;
using System.Collections.Generic;
using System.Linq;
//using System.Media;
using System.Text;
using UnityEngine;
/// <summary>
/// 邪恶术士
/// 同时对两个角色造成100%力量值的伤害，每三回合额外攻击一次
/// 额外攻击会使角色进入两回合虚弱状态，此状态受到的所有伤害增加50%
/// 远程攻击，攻击后不移动
/// </summary>
public class EvilWarlock : Unit
{

    public Player hurtedPlayer
    {
        get;
        private set;
    }
    public EvilWarlock(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "邪恶术士",//精英怪
        Blood = 200,//初始血量为最大血量
        Attack = 25,//攻击力
        Defence = 10,//防御力
        Speed = 2,//先攻权重
    }, pos)
    {
    }

    /// <summary>
    /// 行动
    /// </summary>
    protected override void Decide()
    {
        
        
        //得到要攻击的对象
        List<Player>  p = getAttackPlayer();

        //攻击对象
        attackPlayer(p);

        //撤退
       // retreat(p.Position);

    }
    public List<Player> getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();

        List<Player> attackPlayers = new List<Player>();

        foreach (Player p in players)
        {
            if (p.Position.x >= this.Position.x - 2 && p.Position.x <= this.Position.x + 2
                && p.Position.y >= this.Position.y - 2 && p.Position.y <= this.Position.y + 2)
            {
                attackPlayers.Add(p);
            }

        }
        return attackPlayers;
    }

    /// <summary>
    /// 移动到距离攻击玩家较远的地方
    /// 
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(List<Player> players)
    {

        //移动
        //MoveclosePlayerPos(p.Position);
        //得到当前回合数
        int roundNumber = GameManager.Instance.GetState<BattleState>().RoundNumber;
        foreach (Player p in players)
        {
            if (roundNumber <= 3)

                //100%伤害
                (p as IHurtable).Hurt((int)(this.UnitData.Attack * 1), HurtType.FromUnit, this);



            else//回合数大于等于3次
            {
                //150%伤害
                (p as IHurtable).Hurt(this.UnitData.Attack * 1.5f, HurtType.FromUnit, this);


                //-50%治疗

                (p as ICurable).Cure(this.UnitData.Heal * 0.5f,this);

            }
        }



    }

    /// <summary>
    /// 选择落点
    /// </summary>
    /// <param name="playerPos">玩家位置</param>
    /// <returns></returns>
    public void MoveclosePlayerPos(Vector2Int playerPos)
    {
        //获取可以移动的位置
        List<Vector2Int> moveablePos = GetMoveArea().ToList();
        Vector2Int pos = playerPos;
        bool flag = false;//是否找到可靠近的位置

        //玩家附近有八个位置，找到一个可降落的位置

        pos = new Vector2Int(playerPos.x + 2, playerPos.y + 2);

        //移动到距离玩家较远的地方
        Move(pos);
    }


}