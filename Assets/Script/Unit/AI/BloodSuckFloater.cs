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
    public BloodSuckFloater(Vector2Int pos) : base(new UnitData()
    {
        Name = "BloodSuckFloater",//吸血飞蚊
        BloodMax = 80,//最大血量
        Blood = 80,//初始血量为最大血量
        Attack = 10,//攻击力
        Defence = 4,//防御力
        Speed = 2,//先攻权重
        //移动和技能的使用会消耗技能点,但怪物无行动点约束，设为最大值
        ActionPointMax = int.MaxValue,
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
    }

    /// <summary>
    /// 优先攻击防御最低的角色
    /// </summary>
    /// <returns>要攻击的玩家</returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = -1;//记录防御最低的玩家的索引
        int i = 0;
        double minDifence = int.MaxValue;//设初值为最大值

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
        (player as IHurtable).Hurt((int)(this.UnitData.Attack*0.8), HurtType.FromUnit, this);
        this.UnitData.Blood += (int)(this.UnitData.Attack * 0.8 / 0.5);
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
        bool flag = false;//是否找到可靠近的位置
        //玩家附近有八个位置，找到一个可降落的位置
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
