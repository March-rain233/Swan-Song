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
        DefaultViewType = 1,
        DefaultName = "哥布林盗贼",
        DefaultDescription = "精英\n" +
        "对防御力最低的玩家角色发动攻击，造成120%力量值的伤害并恢复在场所有怪物等于20%造成伤害的血量",
        Blood = 160,
        Attack = 15,
        Defence = 1,
        Speed = 3,
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
    /// 根据玩家防御力最小值，选择合适的攻击对象
    /// </summary>
    /// <returns></returns>
    public Player getAttackPlayer()
    {
        //获得玩家对象
        List<Player> players = GameManager.Instance.GetState<BattleState>().PlayerList.Where(p => p.ActionStatus != ActionStatus.Dead).ToList(); ;
        int num = -1;//记录防御最低的玩家的索引
        int i = 0;
        double minDifence = int.MaxValue;//设初值为最小值

        foreach (Player p in players)
        {
            if (p.UnitData.Defence < minDifence && p.ActionStatus != ActionStatus.Dead)
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

        var damage = (player as IHurtable).Hurt(UnitData.Attack * 1.2f, HurtType.FromUnit | HurtType.AD | HurtType.Melee, this);
        foreach(ICurable unit in GameManager.Instance.GetState<BattleState>().UnitList
            .Where(p=>p.ActionStatus != ActionStatus.Dead && p.Camp == Camp))
        {
            unit.Cure(damage * 0.2f, this);
        }
    }



}