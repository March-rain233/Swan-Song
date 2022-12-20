using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = System.Random;
using Assets.Script.Buff;
/// <summary>
///每当一个其他怪物死亡，此单位获得10%血量和10%力量
///半血以上远程攻击所有目标，并且有20%几率施加混乱状态
///半血以下进入“高速隐蔽”状态，每当有角色进入以自己为中心5x5的范围内，就瞬移到其他地方，并且使得其他怪物力量增加10%
/// </summary>
public class Boss : Unit
{
    const int BloodMax = 300;

    public Boss(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "亡灵古魂",
        Blood = BloodMax,//初始血量为最大血量
        Attack = 20,//攻击力
        Defence = 8,//防御力
        Speed = 2,//先攻权重
        //移动和技能的使用会消耗技能点,但怪物无行动点约束，设为最大值
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
        int curBlood = this.UnitData.Blood;
        if (curBlood < BloodMax / 2)
        {
            Player player = getAttackPlayer();
            //如果身边有人 就瞬移到另一个没人的地方
            retreat(player.Position);
            
            EndTurn();
        }
        else
        {
            //得到要攻击的对象
            List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
            //攻击对象
            attackPlayer(players);
            EndTurn();
        }
    }

    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        Random random = new Random();
        int a = random.Next(5);
        return players[a];
    }
    /// <summary>
    /// 对所有角色造成100%的力量值伤害
    /// 20几率施加混乱状态
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void attackPlayer(List<Player> players)
    {
        foreach (Player player in players)
        {
            //攻击
            (player as IHurtable).Hurt(this.UnitData.Attack * 1.0f, HurtType.FromUnit | HurtType.Ranged | HurtType.AD, this);
            //生成随机数判断是否施加混乱状态
            Random random = new Random();
            int a = random.Next(5);
            if (a == 0)
            {
                //在此处施加混乱
                Confusion conf = new Confusion();
                conf.Time = 3;
                player.AddBuff(conf);
            }


        }

    }

    /// <summary>
    /// 每当有角色进入以自己为中心5x5的范围内，就瞬移到其他地方
    /// </summary>

    public void retreat(Vector2Int playerPos)
    {
        
        int num = 0;
        Random random = new Random();
        int a = random.Next(5);
        //获取可以移动的位置
        List<Vector2Int> moveablePos = GetMoveArea().ToList();
        Vector2Int pos = playerPos;
        bool flag = false;//是否找到可靠近的位置
        
        for (int i = -2; i <= 2 && !flag; ++i)
        {
            for (int j = -2; j <= 2 && !flag; ++j)
            {
                pos = new Vector2Int(playerPos.x+a + i, playerPos.y+a + j);

                foreach (Vector2Int ps in moveablePos)
                {
                    //判断该位置是否可撤退
                    if (pos != ps)
                    {
                        num++;
                        if (num == 25)
                        {
                            flag = true;
                            break;
                        }
                        else continue;

                        
                    }
                }
            }
        }
        Move(pos);

    }


    /// <summary>
    ///半血以下进入“高速隐蔽”状态，其他怪物力量增加10%
    /// </summary>
    public void giveOthersBuff()
    {
        List<Unit> Units = GameManager.Instance.GetState<BattleState>().UnitList.ToList();
        foreach (Unit unit in Units)
        {
            
            if (this.UnitData.Blood < BloodMax / 2)
            {
                Gain gain = new Gain();

                unit.AddBuff(gain);

            }

            
        }
    }

    /// <summary>
    ///每当一个其他怪物死亡，此单位获得10%血量和10%力量
    /// 20几率施加混乱状态
    /// </summary>

    public void giveBossBuff()
    {
        List<Unit> Units = GameManager.Instance.GetState<BattleState>().UnitList.ToList();
        foreach (Unit unit in Units)
        {

            if (this.UnitData.Blood == 0)
            {
                GainBoss gainboss = new GainBoss();

                this.AddBuff(gainboss);

            }


        }
    }
}

