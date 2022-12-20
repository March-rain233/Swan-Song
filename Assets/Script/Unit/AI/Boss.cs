using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = System.Random;
/// <summary>
///每当一个其他怪物死亡，该单位恢复10%血量并提升10%力量
///半血以上远程攻击所有目标，并且有20%几率施加混乱状态
///半血以下进入“高速隐蔽”状态，每当有角色进入以自己为中心5x5的范围内，就瞬移到其他地方，并且使得其他怪物力量增加10%
/// </summary>
public class Boss : Unit
{
    AreaHelper AreaHelper = new AreaHelper()
    {
        Center = new Vector2Int(2, 2),
        Flags = new bool[5, 5]
{
            {true, true, true, true, true},
            {true, true, true, true, true},
            {true, true, true, true, true},
            {true, true, true, true, true},
            {true, true, true, true, true},
}
    };
    public Boss(Vector2Int pos) : base(new UnitModel()
    {
        DefaultViewType = 1,
        DefaultName = "亡灵古魂",
        DefaultDescription = "领主\n" +
        "每当一个其他怪物（不包括衍生物）死亡，该单位恢复10%血量并提升10%力量\n" +
        "半血以上远程攻击所有目标，并且有20%几率施加2回合混乱状态\n" +
        "半血以下进入“高速隐蔽”状态，每当行动时有玩家角色位于以自己为中心5x5的范围内，就瞬移到其他地方，并且使得其他怪物力量增加10%",
        Blood = 300,//初始血量为最大血量
        Attack = 20,//攻击力
        Defence = 40,//防御力
        Speed = 4,//先攻权重
        //移动和技能的使用会消耗技能点,但怪物无行动点约束，设为最大值
        ActionPoint = int.MaxValue,
    }
, pos)
    {
    }

    protected override void OnInit()
    {
        base.OnInit();
        foreach(var u in GameManager.Instance.GetState<BattleState>()
            .UnitList.Where(u=>u.ActionStatus != ActionStatus.Dead && u.Camp == Camp.Enemy))
        {
            u.UnitDied += () =>
            {
                (this as ICurable).Cure(UnitData.BloodMax * 0.1f, this);
                AddBuff(new Gain());
            };
        }
    }
    /// <summary>
    /// 行动
    /// </summary>
    protected override void Decide()
    {
        var map = GameManager.Instance.GetState<BattleState>().Map;
        int curBlood = this.UnitData.Blood;
        if (curBlood < UnitData.BloodMax / 2)
        {
            if(AreaHelper.GetPointList(Position)
                .Where(p=> p.x >= 0 && p.x < map.Width
                && p.y >= 0 && p.y < map.Height)
                .FirstOrDefault(p=>map[p].Units.FirstOrDefault() is Player) != null)
            {
                var posList = map.Where(p => p.tile.Units.Count() == 0 && p.tile.CheckPlaceable(this))
                    .Select(p => p.pos);
                var rand = UnityEngine.Random.Range(0, posList.Count());
                Position = posList.ElementAt(rand);
                foreach(var u in GameManager.Instance.GetState<BattleState>().UnitList.Where(p=> p.ActionStatus != ActionStatus.Dead && p.Camp == Camp))
                {
                    u.AddBuff(new Gain() { });
                }
            }
            //如果身边有人 就瞬移到另一个没人的地方
            
            EndTurn();
        }
        else
        {
            //得到要攻击的对象
            List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList
                .Where(p=>p.ActionStatus != ActionStatus.Dead)
                .ToList();
            //攻击对象
            attackPlayer(players);
            EndTurn();
        }
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
            int a = UnityEngine.Random.Range(0, 6);
            if (a == 0)
            {
                //在此处施加混乱
                Confusion conf = new Confusion();
                conf.Time = 2;
                player.AddBuff(conf);
            }


        }

    }
}

