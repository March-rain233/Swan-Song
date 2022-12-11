using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 骷髅弓手
/// 对一格内的单位发射弓箭，造成100%力量值的伤害，
/// 攻击距离最近的角色，原地进行远程攻击，攻击后不移动。
/// </summary>
public class Skeletonarchers : Unit
{ 
    public Skeletonarchers(Vector2Int pos) : base(new UnitData()
    {
        Name = "Skeletonarchers",//骷髅弓手
        BloodMax = 100,
        Blood = 100,
        Attack = 20,
        Defence = 4,
        Speed = 4,
        //移动和技能的使用会消耗技能点,但怪物无行动点约束，设为最大值
        ActionPointMax = int.MaxValue,
        ActionPoint = int.MaxValue,
    }
, pos)
    {

    }

    protected override void Decide()
    {
        //得到要攻击的对象
        Player player = getAttackPlayer();
        //攻击对象
        attackPlayer(player);
    }
    /// <summary>
    /// 根据玩家距离骷髅弓手的距离，选择合适的攻击对象
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
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
    /// 对一格内的单位发射弓箭，造成100%力量值的伤害
    /// </summary>
    /// <param name="player"></param>
    public void attackPlayer(Player player)
    {
        //远程伤害
        (player as IHurtable).Hurt(this.UnitData.Attack, HurtType.Ranged, this);
    }
}
