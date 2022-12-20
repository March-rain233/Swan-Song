using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 不稳定史莱姆
/// 对一个角色造成100%力量值的伤害
/// 攻击时搜索最近距离(计算规则就按直线距离算)的敌人,
/// 移动至敌人身前进行攻击,攻击后停留在角色周围(5x5的格子内随机)。
/// 死亡后爆炸对周身5x5的格子内敌人造成200%力量值的伤害
/// 
/// </summary>
public class UnstableSlime : Unit
{
    public UnstableSlime(Vector2Int pos) : base(new UnitModel()
    {
        DefaultViewType = 1,
        DefaultName = "不稳定史莱姆",
        DefaultDescription = "普通怪物\n" +
        "对距离最近的角色造成100%力量值的伤害，死亡后发生爆炸对周身5*5范围内所有角色造成200%力量值伤害",
        Blood = 60,//初始血量为最大血量
        Attack = 15,//攻击力
        Defence = 10,//防御力
        Speed = 2,//先攻权重
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
    /// 根据玩家距离不稳定史莱姆的距离，选择合适的还活着的攻击对象
    /// </summary>
    /// <returns>要攻击的玩家</returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.Where(p => p.ActionStatus != ActionStatus.Dead).ToList(); ;
        int num = 0;//记录距离最短的玩家的号码
        int i = 0;
        double minDis = int.MaxValue;//设初值为最大值

        foreach (Player p in players)
        {
            double dis = Math.Pow(Math.Abs(p.Position.x - this.Position.x), 2.0) + Math.Pow(Math.Abs(p.Position.y - this.Position.y), 2.0);
            if (dis < minDis && p.ActionStatus != ActionStatus.Dead)
            {
                minDis = dis;
                num = i;
            }
            i++;
        }
        return players[num];
    }
    /// <summary>
    /// 移动到要攻击玩家附近
    /// 对一个角色造成100的力量值伤害
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(Player player)
    {
        //移动
        MoveclosePlayerPos(player.Position);
        //攻击
        (player as IHurtable).Hurt(this.UnitData.Attack, HurtType.FromUnit | HurtType.Melee | HurtType.AD, this);
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

    /// <summary>
    /// /// 死亡后爆炸对周身5x5的格子内敌人造成200%力量值的伤害
    /// </summary>
    protected override void OnDied()
    {
        List<Unit> players = GameManager.Instance.GetState<BattleState>().UnitList.Where(p => p.ActionStatus != ActionStatus.Dead).ToList(); ;
        //一次判断玩家是否在不稳定史莱姆附近5*5格子内，是就造成200%力量伤害
        foreach(Unit p in players)
        {
            if(p.Position.x <= this.Position.x+2 && p.Position.x >= this.Position.x - 2
                && p.Position.y <= this.Position.y + 2 && p.Position.y >= this.Position.y - 2)
            {
                (p as IHurtable).Hurt(this.UnitData.Attack * 2, HurtType.FromUnit | HurtType.AD, this);
            }
        }
    }

}

