using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// 哥布林抢手
/// 对一个角色造成三次50%力量值的伤害，攻击时搜索最近距离的敌人，
/// 移动至敌人身前进行攻击，攻击后停留在角色周围（5x5的格子内随机)。
/// </summary>
public class Goblingunner : Unit
{
   
    public Goblingunner(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "哥林布枪手",//哥林布枪手
        Blood = 100,//初始血量为最大血量
        Attack = 20,//攻击力
        Defence = 4,//防御力
        Speed = 4,//先攻权重
        ActionPoint = 15,//移动和技能的使用会消耗技能点
    }, pos)
    {
    }

    /// <summary>
    /// 行动
    /// </summary>
    protected override void Decide()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        //得到要攻击的对象
        Player player = getAttackPlayer(players);
        //攻击对象
        attackPlayer(player);
        //撤退
        retreat(player.Position);
    }

    /// <summary>
    /// 根据玩家距离哥布林抢手的距离，选择合适的攻击对象
    /// </summary>
    /// <param name="players">所有玩家</param>
    /// <returns></returns>
    public Player getAttackPlayer(List<Player> players)
    {
        int num = -1;//记录距离最短的玩家的号码
        int i = 0;
        double minDis = int.MaxValue;//设初值为最大值

        foreach (Player p in players)
        {
            double dis = Math.Pow(Math.Abs(p.Position.x - this.Position.x), 2.0) + Math.Pow(Math.Abs(p.Position.y - this.Position.y), 2.0);
            if (dis < minDis && p.ActionStatus == ActionStatus.Running)
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
    /// 对一个角色造成3次50%的力量值伤害
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(Player player)
    {
        //移动
        MoveclosePlayerPos(player.Position);
        //攻击
        for (int i = 0; i < 3; i++)
        {
            (player as IHurtable).Hurt((int)(this.UnitData.Attack * 0.5), HurtType.FromUnit, this);
        }
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
                    if (pos.x == ps.x && pos.y == ps.y)
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
                    if (pos.x == ps.x && pos.y == ps.y)
                    {
                        flag = true;
                        break;
                    }
                }
            }
        }
        Move(pos);
    }

    /*
    /// <summary>
    /// 根据剩余血量，防御力综合选择更适合杀死的对象
    /// </summary>
    /// <param name="players"></param>
    /// <returns></returns>
    public int getAttackPlayerNum(List<Player> players)
    {
        int num = -1;//记录选择攻击的玩家的index
        int i = 0;
        int nearDeath = int.MaxValue;//设初值为最大值

        foreach (Player p in players)
        {
            //当前血量 减去三次攻击，加上三次防御，最接近0的则为最划算的
            int remainBlood = p.UnitData.Blood - this.UnitData.Attack * 3 * 50 % + p.UnitData.Defence * 3;
            
            if (Math.Abs(remainBlood) < nearDeath && p.ActionStatus == ActionStatus.Running)
            {
                nearDeath = Math.Abs(remainBlood);
                num = i;
            }
            i++;
        }
        return num;
    }
    /// <summary>
    ///  进行3次攻击
    /// </summary>
    /// <param name="player"></param>
    public void attackPlayer(Player player)
    {
        for(int i = 0; i < 3; i++)
        {
            (player as IHurtable).Hurt((int)(this.UnitData.Attack * 0.5), HurtType.FromUnit, this);
        }
    }
    */
}
