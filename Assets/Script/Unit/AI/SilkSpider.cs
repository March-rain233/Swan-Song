using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
/// <summary>
///对一个角色喷吐蛛丝，造成20%力量值的伤害，使其本回合无法移动，
///如果一个角色本局对战被同一丝蛛喷吐三次，形成蛛网束缚，
///本局对战剩余时间无法移动(传送类技能除外)，优先攻击体力上限最高的角色，远程攻击，不移动。
/// </summary>
public class SilkSpider : Unit
{

    public SilkSpider(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "丝蛛",
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
    ///  被丝蛛攻击过的玩家
    /// </summary>
    public Dictionary<Unit,int> attackedPlayers = new Dictionary<Unit, int>();
    /// <summary>
    /// 行动
    /// </summary>
    protected override void Decide()
    {
        //得到要攻击的对象
        Player player = getAttackPlayer();
        //攻击对象
        attackPlayer(player);
    }

    /// <summary>
    /// 优先攻击体力上限最高的角色
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = -1;
        int i = 0;
        double maxAction = int.MinValue;//设初值为最小值

        foreach (Player p in players)
        {
            if (p.UnitData.ActionPoint > maxAction && p.ActionStatus == ActionStatus.Running)
            {
                maxAction = p.UnitData.ActionPoint;
                num = i;
            }
            i++;
        }
        return players[num];
    }
    /// <summary>
    /// 对一个角色造成20%的力量值伤害
    /// 使其本回合无法移动
    /// 如果一个角色本局对战被同一丝蛛喷吐三次，形成蛛网束缚
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(Player player)
    {
        //攻击
        (player as IHurtable).Hurt(this.UnitData.Attack * 0.2f, HurtType.FromUnit | HurtType.Ranged | HurtType.AD, this);
        Restrain restrain = new Restrain();
        restrain.Time = 1;
        player.AddBuff(restrain);

        bool attacked = false;//是否被攻击过
        foreach(var p in attackedPlayers.Keys)
        {
            if(p == player)
            {
                attacked = true;
                attackedPlayers[p]++;
                if(attackedPlayers[p] == 3)
                {
                    restrain.Time = int.MaxValue;//不能移动的buff施加时间无限长，即本局都不能动
                }
                break;
            }
        }
        if (!attacked)
        {
            attackedPlayers.Add(player,1);
        }

    }


}

