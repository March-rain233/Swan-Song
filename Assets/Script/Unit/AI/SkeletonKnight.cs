using System;
using System.Collections.Generic;
using System.Linq;
//using System.Media;
using System.Text;
using UnityEngine;
/// <summary>
/// 骷髅骑士
/// 攻击冲撞一个敌人对其造成100%力量值伤害，并使本回合剩余时间晕眩
/// 骑士半血以下只进行穿刺，对一个角色造成200%的力量伤害
/// 优选攻击最远角色，攻击时移动到身前，攻击后不动
/// </summary>
public class SkeletonKnight : Unit
{
    public SkeletonKnight(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "骷髅骑士",
        Blood = 200,
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
    }

    /// <summary>
    /// 根据玩家距离骷髅骑士的距离，选择合适的攻击对象
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = -1;//记录距离最远的玩家的号码
        int i = 0;
        double maxDis = int.MaxValue;//设初值为最大值

        foreach (Player p in players)
        {
            double dis = Math.Pow(Math.Abs(p.Position.x - this.Position.x), 2.0) + Math.Pow(Math.Abs(p.Position.y - this.Position.y), 2.0);
            if (dis > maxDis && p.ActionStatus == ActionStatus.Running)
            {
                maxDis = dis;
                num = i;
            }
            i++;
        }
        return players[num];
    }

    /// <summary>
    ///  进行范围攻击
    ///  攻击冲撞一个敌人对其造成100%力量值伤害，并使本回合剩余时间晕眩
    /// 骑士半血以下只进行穿刺，对一个角色造成200%的伤害
    /// </summary>
    /// <param name="player"></param>
    public void attackPlayer(Player player)
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int standard = (int)(int.MaxValue * 0.5);
        foreach (Player p in players)
        {

            //如果怪物在半血以下
            if (p.UnitData.Blood < standard )
            {
                if (p.Position.x <= player.Position.x + 1 && p.Position.x >= player.Position.x - 1
                    && p.Position.y <= player.Position.y + 1 && p.Position.y >= player.Position.y - 1)
                {
                    //近身冲刺伤害
                    (p as IHurtable).Hurt((int)(this.UnitData.Attack * 2), HurtType.FromUnit | HurtType.Ranged | HurtType.AD, this);
                }
                else continue;
            }

            else//否则攻击不加成且造成晕眩本回合
            {
                Stun stun = new Stun();
                stun.Time = 1;
                p.AddBuff(stun);
                (p as IHurtable).Hurt((int)(this.UnitData.Attack * 1), HurtType.FromUnit | HurtType.Ranged | HurtType.AD, this);
            }
        }
    }

}