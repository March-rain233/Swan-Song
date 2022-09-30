using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 卡牌调度器
/// </summary>
/// <remarks>用来管理一个单位在战斗过程中的卡牌</remarks>
public class CardScheduler
{
    /// <summary>
    /// 弃牌区
    /// </summary>
    /// <remarks>释放或被遗弃的牌</remarks>
    public List<Card> DiscardPile
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 手牌区
    /// </summary>
    /// <remarks>已被抽取的牌</remarks>
    public List<Card> Hands
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 牌堆
    /// </summary>
    /// <remarks>还未抽取的牌</remarks>
    public List<Card> Deck
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 持有该调度器的单位
    /// </summary>
    public Unit Unit
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 释放手牌中对应索引的卡牌
    /// </summary>
    public void ReleaseCard(int index)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 抽一张卡
    /// </summary>
    /// <returns>抽取到的卡牌</returns>
    public Card DrawCard()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 回合开始初始化
    /// </summary>
    internal void Prepare()
    {
        throw new System.NotImplementedException();
    }
}