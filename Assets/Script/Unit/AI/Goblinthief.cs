using System;
using System.Collections.Generic;
using System.Linq;
//using System.Media;
using System.Text;
using UnityEngine;
/// <summary>
/// 哥布林盗贼
/// 对周围8格造成120&伤害，所有攻击优先选择防御最低
/// 每三回合随机选择一个角色，以其为中心，在3*3方格内制造造毒雾，持续三回合（远程攻击）
/// 每回合对对敌人造成5%最大生命值的伤害，窃取对角色伤害的20%，到怪物血量
/// 近战攻击，移动到角色身前攻击，随后在场上随机游走
/// 攻击时移动到身前，攻击后不动
/// </summary>
public class GoblinThief : Unit
{
    public GoblinThief(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "哥布林盗贼",
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
    /// 根据玩家防御力最小值，选择合适的攻击对象
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        int num = -1;//记录防御最低的玩家的索引
        int i = 0;
        double minDifence = int.MaxValue;//设初值为最小值

        foreach (Player p in players)
        {
            if (p.UnitData.Defence < minDifence && p.ActionStatus == ActionStatus.Running)
            {
                minDifence = p.UnitData.Defence;
                num = i;
            }
            i++;
        }
        return players[num];
    }

    /// <summary>
    /// 靠近玩家
    /// </summary>
    /// <param name="playerPos">玩家位置</param>
    /// <returns></returns>
    public void MoveclosePlayerPos(Vector2Int playerPos)
    {
        //获取可以移动的位置
        List<Vector2Int> moveablePos = GetMoveArea().ToList();
        Vector2Int pos = playerPos;
        bool flag = false;//是否找到可靠近的位置
        //玩家附近有八个位置，找到一个可降落的位置
        for (int i = -1; i <= 1 && !flag; ++i)
        {
            for (int j = -1; j <= 1 && !flag; ++j)
            {
                pos = new Vector2Int(playerPos.x + i, playerPos.y + j);

                foreach (Vector2Int ps in moveablePos)
                {
                    if (pos == ps)
                    {
                        flag = true;
                        break;
                    }
                }
            }
        }
        //移动到玩家附近
        Move(pos);
    }

    /// <summary>
    /// 对周围8格造成120&伤害
    /// 每三回合随机选择一个角色，以其为中心，在3*3方格内制造造毒雾，持续三回合（远程攻击）
    /// 每回合对对敌人造成5%最大生命值的伤害，窃取对角色伤害的20%，到怪物血量
    /// </summary>
    /// <returns></returns>


    public void attackPlayer(Player player)
    {
        //移动
        MoveclosePlayerPos(player.Position);

        //中毒buff
        Poison poison = new Poison();
        poison.Time = 3;
        poison.Damage = this.UnitData.Attack * 0.05f;
        player.AddBuff(poison);

        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.ToList();
        foreach (Player p in players)
        {
            if (p.Position.x <= player.Position.x + 1 && p.Position.x >= player.Position.x - 1
             && p.Position.y <= player.Position.y + 1 && p.Position.y >= player.Position.y - 1
                )
            {
                //近身伤害
                (p as IHurtable).Hurt((int)(this.UnitData.Attack * 1.2), HurtType.FromUnit | HurtType.Melee | HurtType.AD, this);

                //为其他怪物回血 
                List<Unit> Units = GameManager.Instance.GetState<BattleState>().UnitList.ToList();
                foreach (Unit unit in Units)
                {
                    (unit as ICurable).Cure(poison.Damage*0.2f,unit);
                }

            }
        }
    }



}