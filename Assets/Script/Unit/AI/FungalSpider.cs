using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 剧毒蜘蛛: 
/// 对一个角色造成50%力量值的伤害，
/// 优先攻击血量最少的敌人，
/// 并附带三回合的中毒效果，
/// 每回合造成10%力量值的伤害，该伤害无视防御，
/// 死亡后原地3x3的范围生成毒液地形，
/// 触碰到的敌人施加上述中毒效果，毒液地形存在两回合，
/// 移动至敌人身前进行攻击,攻击后停留在角色周围(5x5的格子内随机)。
/// </summary>
public class FungalSpider : Unit
{
    public FungalSpider(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "剧毒蜘蛛",
        Blood = 80,//初始血量为最大血量
        Attack = 10,//攻击力
        Defence = 4,//防御力
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
    /// 优先攻击血量最少的敌人
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = -1,i = 0;//记录血量最少的玩家索引
        int lessBlood = int.MaxValue;//设初值为最大值

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
    /// 移动到要攻击玩家附近
    /// 对一个角色造成50%的力量值伤害
    /// 附带三回合的中毒效果，
    /// 每回合造成10%力量值的伤害，该伤害无视防御
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(Player player)
    {
        //移动
        MoveclosePlayerPos(player.Position);
        //攻击
        (player as IHurtable).Hurt(this.UnitData.Attack * 0.5f, HurtType.FromUnit | HurtType.Melee | HurtType.AD, this);
        //中毒buff
        Poison poison = new Poison();
        poison.Time = 3;
        poison.Damage = this.UnitData.Attack * 0.1f;
        player.AddBuff(poison);
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

    /// <summary>
    /// 死亡后原地3x3的范围生成毒液地形
    /// 触碰到的敌人施加上述中毒效果，毒液地形存在两回合
    /// </summary>
    protected override void OnDied()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        Poison poison = new Poison();
        poison.Time = 3;
        poison.Damage = this.UnitData.Attack * 0.1f;
        foreach (Player p in players)
        {
            if (p.Position.x <= this.Position.x + 1 && p.Position.x >= this.Position.x - 1
             && p.Position.y <= this.Position.y + 1 && p.Position.y >= this.Position.y - 1
                )
            {
                //添加中毒buff
                p.AddBuff(poison);
            }
        }
    }
    
}
