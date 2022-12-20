using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 剧毒蜘蛛: 
/// 对一个角色造成50%力量值的伤害，
/// 优先攻击血量最少的敌人，
/// 并附带三回合的中毒效果，
/// 每回合造成10%力量值的伤害，该伤害无视防御
/// </summary>
public class FungalSpider : Unit
{
    public AreaHelper AreaHelper = new AreaHelper()
    {
        Center = new Vector2Int(1, 1),
        Flags = new bool[3, 3]
        {
            {true, true, true },
            {true, true, true },
            {true, true, true }
        }
    };
    public FungalSpider(Vector2Int pos) : base(new UnitModel()
    {
        DefaultViewType = 1,
        DefaultName = "剧毒蜘蛛",
        DefaultDescription = "普通怪物\n" +
        "对血量最少的敌人造成50%力量值的伤害，并施加3回合中毒\n" +
        "死后发生爆裂，将周围方格变为2回合的毒液地形",
        Blood = 40,//初始血量为最大血量
        Attack = 12,//攻击力
        Defence = 1,//防御力
        Speed = 3,//先攻权重
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
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList
            .Where(p=>p.ActionStatus!= ActionStatus.Dead).ToList();
        int num = 0,i = 0;//记录血量最少的玩家索引
        int lessBlood = int.MaxValue;//设初值为最大值

        foreach (Player p in players)
        {
            if(p.UnitData.Blood < lessBlood && p.ActionStatus != ActionStatus.Dead)
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
    /// 死亡后原地3x3的范围生成毒液地形
    /// 触碰到的敌人施加上述中毒效果，毒液地形存在两回合
    /// </summary>
    protected override void OnDied()
    {
        var map = GameManager.Instance.GetState<BattleState>()
            .Map;
        //获得玩家对象
        foreach(var t in AreaHelper.GetPointList(Position)
            .Where(p=>p.x >= 0 && p.x < map.Width 
            && p.y >= 0 && p.y < map.Height)
            .Select(p => map[p]))
        {
            t.AddStatus(new PoisonStatus() { Times = 2 });
        }
    }
    
}
