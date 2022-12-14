using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 吸血飞蚊
/// 对一个角色造成80%力量值的伤害，并恢复自身造成伤害的五分之一的血量，
/// 优先攻击防御最低的角色，移动至角色身前进行攻击，攻击后停留在角色周围(5x5的格子内随机)。
/// </summary>
public class BloodSuckFloater :Unit
{
    public BloodSuckFloater(Vector2Int pos) : base(new UnitModel()
    {
        DefaultViewType = 1,
        DefaultName = "吸血飞蚊",
        DefaultDescription = "普通小怪\n" +
        "对防御力最低的角色造成80%力量值的伤害，并恢复自身造成伤害的五分之一的血量",
        Blood = 50,//初始血量为最大血量
        Attack = 10,//攻击力
        Defence = 10,//防御力
        Speed = 5,//先攻权重
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
    /// 优先攻击防御最低的角色
    /// </summary>
    /// <returns>要攻击的玩家</returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList
            .Where(p=>p.ActionStatus != ActionStatus.Dead).ToList();
        int num = 0;//记录防御最低的玩家的索引
        int i = 0;
        double minDifence = int.MaxValue;//设初值为最大值

        foreach (Player p in players)
        { 
            if (p.UnitData.Defence < minDifence && p.ActionStatus != ActionStatus.Dead)
            {
                minDifence = p.UnitData.Defence;
                num = i;
            }
            i++;
        }
        return players[num];
    }
    /// <summary>
    /// 移动到要攻击玩家附近
    /// 对一个角色造成80的力量值伤害
    /// 恢复自身造成伤害的五分之一的血量
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(Player player)
    {
        //移动
        MoveclosePlayerPos(player.Position);
        //攻击
        var data = (player as IHurtable).Hurt((int)(this.UnitData.Attack*0.8), HurtType.FromUnit | HurtType.Melee | HurtType.AD, this);
        (this as ICurable).Cure(data * 0.2f, this);
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
