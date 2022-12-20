using System;
using System.Collections.Generic;
using System.Linq;
//using System.Media;
using System.Text;
using UnityEngine;
/// <summary>
/// 邪恶术士
/// 同时对两个角色造成100%力量值的伤害，每三回合额外攻击一次
/// 额外攻击会使角色进入两回合虚弱状态，此状态受到的所有伤害增加50%
/// 远程攻击，攻击后不移动
/// </summary>
public class EvilWarlock : Unit
{

    public Player hurtedPlayer
    {
        get;
        private set;
    }
    public EvilWarlock(Vector2Int pos) : base(new UnitModel()
    {
        DefaultViewType = 1,
        DefaultName = "邪恶术士",//精英怪
        DefaultDescription = "精英\n" +
        "同时对两个角色造成100%力量值的远程伤害，每三回合发动一次特殊攻击" +
        "特殊攻击会使角色进入两回合虚弱状态",
        Blood = 150,//初始血量为最大血量
        Attack = 18,//攻击力
        Defence = 10,//防御力
        Speed = 1,//先攻权重
    }, pos)
    {
    }

    /// <summary>
    /// 行动
    /// </summary>
    protected override void Decide()
    {
        //得到要攻击的对象
        List<Player>  p = getAttackPlayer();

        //攻击对象
        attackPlayer(p);
        EndTurn();
    }
    public List<Player> getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList
            .Where(p=>p.ActionStatus != ActionStatus.Dead)
            .Take(2).ToList();
        return players;
    }

    public void attackPlayer(List<Player> players)
    {
        int roundNumber = GameManager.Instance.GetState<BattleState>().RoundNumber;
        foreach (Player p in players)
        {
            if (roundNumber % 3 == 0 && roundNumber != 0)
            {
                p.AddBuff(new Weak() { Time = 2 });
            }
                //100%伤害
            (p as IHurtable).Hurt((int)(this.UnitData.Attack * 1), HurtType.AP, this);
        }
    }
}