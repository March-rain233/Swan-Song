using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elite : Unit
{

    public List<Player> PlayerList
    {
        get;
        set;
    } = new();
    public elite(Vector2Int pos) : base(new UnitModel()
    {
        DefaultName = "精英1号",
        Blood = 80,//初始血量为最大血量
        Attack = 10,//攻击力
        Defence = 4,//防御力
        Speed = 3,//先攻权重
        ActionPoint = 20,//角色的移动和技能的使用会消耗技能点
    }, pos)
    {
        Console.WriteLine("精英被创建，加入战场。");
    }
    //Vector2Int _position = Vector2Int.zero;//默认0

    //public Vector2Int positionMove
    //{
    //    get => _position;
    //    set
    //    {
    //        var manager = GameToolKit.ServiceFactory.Instance.GetService<GameManager>()
    //            .GetState() as BattleState;
    //        manager.Map[_position.x+1, _position.y+1].Exit(this);
    //        _position = value;
    //        manager.Map[_position.x+1, _position.y+1].Enter(this);
    //    }
    //}
    //回合制
    protected override void Decide()
    {
        //移动精英 根据和玩家的距离确定移动位置？
        //Move(this.Position + this.positionMove);//当前位置 + 移动位置 = 移动后的位置

        //释放卡牌
        List<Card> cards = this.UnitData.Deck;
        //根据一定的策略选择要释放卡牌，得到卡牌号码rcn
        int rcn = getNumCardBest(cards);
        //foreach (Card c in cards)
        //{
        //    c.Release(this, this.Position);
        //}    
    }
    //结束回合
    public void EndDecide()
    {
        EndTurn();
    }
    //综合行动点、当前卡牌的攻击力、卡牌数量，综合选择更适合释放的卡牌
    public int getNumCardBest(List<Card>  cards)
    {
        int num = -1;
        //......
        return num;
    }
}
