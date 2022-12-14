using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
///第一回合丢蛛丝，把所有角色拉到自己攻击范围内，并且不允许他们走出去
///如后续对攻击范围内所有角色造成伤害
///死亡后清除牢笼状态
/// </summary>
public class MagicSpider : Unit
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
    public MagicSpider(Vector2Int pos) : base(new UnitModel()
    {
        DefaultViewType = 1,
        DefaultName = "魔蛛",
        DefaultDescription = "精英\n" +
        "第一次行动时喷射蛛丝产生蛛丝场地并将玩家角色拉入场地，进入场地的角色将无法脱离场地，场地将在该单位死亡时清除。" +
        "后续对攻击范围内所有角色造成伤害",
        Blood = 100,//初始血量为最大血量
        Attack = 10,//攻击力
        Defence = 20,//防御力
        Speed = 1,//先攻权重
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
        EndTurn();
    }

    /// <summary>
    /// 第一回合施加buff
    /// </summary>
    /// <param name="player">要攻击的玩家</param>
    public void giveDebuff()
    {
        var map = GameManager.Instance.GetState<BattleState>().Map;
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.Where(p => p.ActionStatus != ActionStatus.Dead).ToList(); ;
        foreach (Player player in players)
        {
            //对所有角色施加牢笼buff

            Vector2Int pos;
            try
            {
                pos = AreaHelper.GetPointList(Position)
                    .First(p => p.x >= 0 && p.x < map.Width
                        && p.y >= 0 && p.y < map.Height &&
                        map[p].Units.Count <= 0 && map[p].CheckPlaceable(player));
            }
            catch (Exception ex)
            {
                break;
            }
            player.Position = pos;
        }
        foreach (var t in AreaHelper.GetPointList(Position)
                .Where(p => p.x >= 0 && p.x < map.Width
                    && p.y >= 0 && p.y < map.Height)
                .Select(p => map[p]))
        {
            t.AddStatus(new SilkscreenStatus());
        }

    }

    /// <summary>
    /// 选取攻击范围内的所有角色
    /// </summary>
    /// <returns></returns>
    public List<Player> getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.Where(p => p.ActionStatus != ActionStatus.Dead).ToList(); ;
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
        var map = GameManager.Instance.GetState<BattleState>().Map;
        foreach (var t in AreaHelper.GetPointList(Position)
        .Where(p => p.x >= 0 && p.x < map.Width
            && p.y >= 0 && p.y < map.Height)
        .Select(p => map[p]))
        {
            t.RemoveStatus<SilkscreenStatus>();
        }
    }

}
