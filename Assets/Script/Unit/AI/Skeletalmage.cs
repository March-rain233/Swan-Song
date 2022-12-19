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
        DefaultName = "骷髅法师",
        Blood = 100,
        Attack = 20,
        Defence = 4,
        Speed = 4,
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
        EndTurn();
    }

    /// <summary>
    /// 根据玩家距离骷髅法师的距离，选择合适的攻击对象
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = 0;//记录距离最短的玩家的号码
        int i = 0;
        double minDis = int.MaxValue;//设初值为最大值

        foreach (Player p in players)
        {
            double dis = Math.Pow(Math.Abs(p.Position.x - this.Position.x), 2.0) + Math.Pow(Math.Abs(p.Position.y - this.Position.y), 2.0);
            if (dis < minDis && p.ActionStatus != ActionStatus.Running)
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
    public void attackPlayer(Player player)
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();

        foreach (Player p in players)
        {
            if (p.Position.x <= player.Position.x + 1 && p.Position.x >= player.Position.x - 1
             && p.Position.y <= player.Position.y + 1 && p.Position.y >= player.Position.y - 1
                )
            {
                //近身伤害
                (p as IHurtable).Hurt((int)(this.UnitData.Attack * 0.8), HurtType.FromUnit | HurtType.Ranged | HurtType.AD, this);
            }
        }
    }

}
