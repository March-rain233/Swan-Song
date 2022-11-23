using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameToolKit;

/// <summary>
/// 卡牌对象
/// </summary>
public abstract class Card
{
    /// <summary>
    /// 卡牌类型
    /// </summary>
    /// <remarks>
    /// 用于给AI判断卡牌类型来进行出牌逻辑
    /// </remarks>
    [Flags]
    public enum CardType
    {
        Other,
        Attack = 1,
        Defence = 1 << 1,
        Heal = 1 << 2,
    }

    /// <summary>
    /// 卡牌释放目标数据
    /// </summary>
    public struct TargetData
    {
        /// <summary>
        /// 可作为目标的图块坐标
        /// </summary>
        public IEnumerable<Vector2Int> AvaliableTile;
        /// <summary>
        /// 技能释放范围的坐标
        /// </summary>
        public IEnumerable<Vector2Int> ViewTiles;
    }
    /// <summary>
    /// 卡牌名称
    /// </summary>
    public string Name;

    /// <summary>
    /// 卡牌简介
    /// </summary>
    public string Description;

    /// <summary>
    /// 卡面图片
    /// </summary>
    public Sprite Sprite;

    /// <summary>
    /// 卡牌的行动费用
    /// </summary>
    public int Cost;

    /// <summary>
    /// 数值比例
    /// </summary>
    public float Percent;

    /// <summary>
    /// 回合数
    /// </summary>
    public int Times = 0;

    /// <summary>
    /// 是否已被强化
    /// </summary>
    public bool HasEnchanted;

    /// <summary>
    /// 卡牌类型
    /// </summary>
    public virtual CardType Type => CardType.Other;

    /// <summary>
    /// 词条
    /// </summary>
    public HashSet<Entry> Entries = new();

    protected Map _map => (ServiceFactory.Instance.GetService<GameManager>().GetState() as BattleState).Map;

    /// <summary>
    /// 释放卡牌
    /// </summary>
    protected internal abstract void Release(Unit user, Vector2Int target);

    /// <summary>
    /// 获取可作用的目标
    /// </summary>
    /// <remarks>
    /// 即可以作为直接作用的目标
    /// </remarks>
    /// <returns></returns>
    protected internal abstract TargetData GetAvaliableTarget(Unit user);

    /// <summary>
    /// 获取影响范围
    /// </summary>
    /// <remarks>
    /// 即Aoe范围或是连锁单位等目标
    /// </remarks>
    /// <returns></returns>
    protected internal abstract IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target);

    /// <summary>
    /// 复制卡牌对象
    /// </summary>
    public virtual Card Clone()
    {
        var card = MemberwiseClone() as Card;
        card.Entries = new HashSet<Entry>(card.Entries);
        return card;
    }
}