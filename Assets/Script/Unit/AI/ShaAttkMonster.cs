using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// 影袭怪
/// 对一个角色造成100%力量值的伤害，如果连续攻击同一角色，
/// 从第二次开始，每次造成150%力量值的伤害
/// （比如第一回合打了角色A，第二回合还打的角色A，那伤害会提升，中间转移过目标后要重新计算)，
/// 优先攻击血量百分比最低的敌人，移动至角色身前进行攻击，攻击后返回原位置。
/// </summary>
public class ShaAttkMonster : Unit
{
    /// <summary>
    /// 上一回合伤害的角色
    /// </summary>
    public Player hurtedPlayer
    {
        get;
        private set;
    }
    public ShaAttkMonster(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "影袭怪", 
        Blood = 80,//初始血量为最大血量
        Attack = 10,//攻击力
        Defence = 4,//防御力
        Speed = 2,//先攻权重
        //移动和技能的使用会消耗技能点,但怪物无行动点约束，设为最大值
        ActionPoint = int.MaxValue,
    }
, pos)
    {
    }

    /// <summary>
    /// 行动
    /// </summary>
    protected override void Decide()
    {  
        //得到要攻击的对象
        Player player = getAttackPlayer();
        //攻击对象
        attackPlayer(player);
        //撤退
        retreat(player.Position);
        EndTurn();
    }

    /// <summary>
    /// 根据血量，选择合适的攻击对象
    /// </summary>
    /// <returns>要攻击的玩家</returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = 0;//记录血量比最少的玩家的号码
        int i = 0;
        double minbloodPercent = int.MaxValue;//设初值为最大值

        foreach (Player p in players)
        {
            double bloodPercent = (double)p.UnitData.Blood / p.UnitData.BloodMax;
            if (bloodPercent < minbloodPercent && p.ActionStatus != ActionStatus.Dead)
            {
                minbloodPercent = bloodPercent;
                num = i;
            }
            i++;
        }
        return players[num];
    }
    /// <summary>
    /// 移动到要攻击玩家附近
    /// 对一个角色造成100%力量值的伤害，如果连续攻击同一角色，
    /// 从第二次开始，每次造成150%力量值的伤害
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(Player player)
    {
        //移动
        MoveclosePlayerPos(player.Position);
        //得到当前回合数
        int roundNumber = GameManager.Instance.GetState<BattleState>().RoundNumber;
        //不是第一回合，影袭怪已经攻击过，判断当前回合攻击玩家是否是上一轮攻击过的对象
        if (roundNumber != 1 && this.hurtedPlayer == player)
        {
            //150%近身伤害
            (player as IHurtable).Hurt((int)(this.UnitData.Attack*1.5), HurtType.FromUnit | HurtType.Melee | HurtType.AD, this);
        }
        else//是第一回合或两次攻击对象不一样
        {
            //100%近身伤害
            (player as IHurtable).Hurt(this.UnitData.Attack, HurtType.FromUnit | HurtType.Melee | HurtType.AD, this);
        }
        this.hurtedPlayer = player;
       
    }

    /// <summary>
    /// 靠近玩家
    /// </summary>
    /// <param name="playerPos">玩家位置</param>
    /// <returns></returns>
    public void MoveclosePlayerPos(Vector2Int playerPos)
    {
        //获取可以移动的位置
        List<Vector2Int> moveablePos = GetMoveArea().ToList();
        Vector2Int pos = playerPos;
        if (playerPos.y - this.Position.y != 0 && playerPos.x - this.Position.x != 0)
        {
            int k = (playerPos.y - this.Position.y) / (playerPos.x - this.Position.x);
            int signal = 0;
            if (playerPos.x > this.Position.x) signal = -1;
            else signal = 1;
            bool find = false;
            for (int i = 0; i < Math.Abs(playerPos.x - this.Position.x) && !find; i++)
            {
                pos.x += signal * (i + 1);
                pos.y += (i + 1) * signal * k;
                foreach (Vector2Int ps in moveablePos)
                {
                    if (pos == ps)
                    {
                        find = true;
                        break;
                    }
                }
            }
        }
        if (playerPos.y - this.Position.y == 0)
        {
            bool find = false;
            for (int i = 0; i < Math.Abs(playerPos.x - this.Position.x) && !find; i++)
            {
                if (playerPos.x > this.Position.x) pos.x -= 1;
                else pos.x += 1;
                foreach (Vector2Int ps in moveablePos)
                {
                    if (pos == ps)
                    {
                        find = true;
                        break;
                    }
                }
            }
        }
        if (playerPos.x - this.Position.x == 0)
        {
            bool find = false;
            for (int i = 0; i < Math.Abs(playerPos.y - this.Position.y) && !find; i++)
            {
                if (playerPos.y > this.Position.y) pos.y -= 1;
                else pos.y += 1;
                foreach (Vector2Int ps in moveablePos)
                {
                    if (pos == ps)
                    {
                        find = true;
                        break;
                    }
                }
            }
        }
        //移动到玩家附近
        Move(pos);
    }
    /// <summary>
    /// 撤退到玩家附近5*5格子内
    /// </summary>
    /// <param name="playerPos">被攻击的玩家的位置</param>
    public void retreat(Vector2Int playerPos)
    {
        //获取可以移动的位置
        List<Vector2Int> moveablePos = GetMoveArea().ToList();
        Vector2Int pos = playerPos;
        bool flag = false;//是否找到可靠近的位置
        //玩家附近有八个位置，找到一个可降落的位置
        for (int i = -2; i <= 2 && !flag; ++i)
        {
            for (int j = -2; j <= 2 && !flag; ++j)
            {
                pos = new Vector2Int(playerPos.x + i, playerPos.y + j);

                foreach (Vector2Int ps in moveablePos)
                {
                    //判断该位置是否可撤退
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
