using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
///第一回合丢垃圾，把所有角色拉到自己攻击范围内，并且不允许他们走出去
///如后续对攻击范围内所有角色造成伤害
///死亡后清除牢笼状态
/// </summary>
public class MagicSpider : Unit
{

    public MagicSpider(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "魔蛛",
        Blood = 100,//初始血量为最大血量
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
    public Dictionary<Unit, int> attackedPlayers = new Dictionary<Unit, int>();
    /// <summary>
    /// 行动
    /// </summary>
    protected override void Decide()
    {
        int roundNumber = GameManager.Instance.GetState<BattleState>().RoundNumber;
        if(roundNumber==1)
        {
            giveDebuff();
        }
        else
        {
            //得到要攻击的对象
            List<Player> players = getAttackPlayer();
            //攻击对象
            attackPlayer(players);
        }    
    }

    /// <summary>
    /// 第一回合施加buff
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void giveDebuff()
    {
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        foreach(Player player in players)
        {
            //对所有角色施加牢笼buff

        }

    }

    /// <summary>
    /// 选取攻击范围内的所有角色
    /// </summary>
    /// <returns></returns>
    public List<Player> getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        List<Player> attackPlayers = new List<Player>();
        foreach (Player p in players)
        {
            if (p.Position.x>=this.Position.x-2&&p.Position.x<=this.Position.x+2
                && p.Position.y >= this.Position.y - 2 && p.Position.y <= this.Position.y + 2)
            {
                attackPlayers.Add(p);
            }
            
        }
        return attackPlayers;
    }
    /// <summary>
    /// 对所有角色造成150%的力量值伤害
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(List<Player> player)
    {
        foreach (Player p in player)
        {
            //攻击
            (p as IHurtable).Hurt(this.UnitData.Attack * 1.5f, HurtType.FromUnit | HurtType.Ranged | HurtType.AD, this);
        }
    }

    protected override void OnDied()
    {
        //死后解除所有角色的牢笼
    }

}
