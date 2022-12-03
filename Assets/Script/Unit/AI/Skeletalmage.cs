using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 骷髅法师
/// 对一个3x3方格内的所有敌方角色造成80%力量值的伤害，
/// 攻击距离最近的角色，原地进行远程攻击，攻击后不移动。
/// </summary>
public class Skeletalmage : Unit
{
    public Skeletalmage(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "骷髅法师",//骷髅法师
        Blood = 100,
        Attack = 20,
        Defence = 4,
        Speed = 4,
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
        attackPlayer(players, player);
    }

    /// <summary>
    /// 根据玩家距离骷髅法师的距离，选择合适的攻击对象
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
    ///  进行范围攻击
    ///  对一个3x3方格内的所有敌方角色造成80%力量值的伤害
    /// </summary>
    /// <param name="player"></param>
    public void attackPlayer(List<Player> players, Player player)
    {
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                int index = hasPlayer(players, new Vector2Int(player.Position.x + i, player.Position.y + j));
                if (index != -1)//如果该位置有角色进行攻击
                {
                    (players[index] as IHurtable).Hurt((int)(this.UnitData.Attack * 0.8), HurtType.FromUnit, this);
                }
            }
        }
    }
    /// <summary>
    /// 判断是否有玩家在这个位置上
    /// </summary>
    /// <param name="players"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int hasPlayer(List<Player> players, Vector2Int pos)
    {
        int i = 0;
        foreach (Player player in players)
        {
            if (player.Position == pos)
            {
                return i;
            }
            i++;
        }
        return -1;
    }
}
