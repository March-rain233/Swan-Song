﻿using System;
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
    /// 卡牌释放目标数据
    /// </summary>
    public struct TargetData
    {
        public IEnumerable<Unit> Units;
        public IEnumerable<Vector2Int> Tiles;
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
    /// 是否已被强化
    /// </summary>
    public bool HasEnchanted;

    /// <summary>
    /// 词条
    /// </summary>
    public HashSet<Entry> Entries
    {
        get;
        internal set;
    } = new();

    protected Map _map => (ServiceFactory.Instance.GetService<GameManager>().GetState() as BattleState).Map;

    /// <summary>
    /// 释放卡牌
    /// </summary>
    protected internal abstract void Release(Unit user, TargetData targetData);

    /// <summary>
    /// 获取可作用的目标
    /// </summary>
    /// <remarks>
    /// 即可以作为直接作用的目标
    /// </remarks>
    /// <returns></returns>
    protected internal abstract TargetData GetAvaliableTarget(Unit user);

    /// <summary>
    /// 获取作用范围
    /// </summary>
    /// <remarks>
    /// 即Aoe范围或是连锁单位等目标
    /// </remarks>
    /// <returns></returns>
    protected internal abstract TargetData GetActionRange(Unit user, TargetData targetData);

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