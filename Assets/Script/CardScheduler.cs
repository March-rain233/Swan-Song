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
    /// <remarks>释放或被遗弃的牌，回合开始时将被洗入牌库</remarks>
    public List<Card> DiscardPile
    {
        get;
        private set;
    } = new List<Card>();

    /// <summary>
    /// 手牌区
    /// </summary>
    /// <remarks>已被抽取的牌</remarks>
    public List<Card> Hands
    {
        get;
        private set;
    } = new List<Card>();

    /// <summary>
    /// 牌堆
    /// </summary>
    /// <remarks>还未抽取的牌</remarks>
    public List<Card> Deck
    {
        get;
        private set;
    } = new List<Card>();

    /// <summary>
    /// 持有该调度器的单位
    /// </summary>
    public Unit Unit
    {
        get;
        internal set;
    }

    internal CardScheduler(Unit unit, List<Card> cards)
    {
        Unit = unit;
        Deck = new List<Card>(cards);
    }

    /// <summary>
    /// 释放手牌中对应索引的卡牌
    /// </summary>
    public void ReleaseCard(int index)
    {
        Hands[index].Release();
        DiscardPile.Add(Hands[index]);
        Hands.RemoveAt(index);
    }

    /// <summary>
    /// 抽一张卡
    /// </summary>
    /// <returns>抽取到的卡牌</returns>
    public Card DrawCard()
    {
        if(Deck.Count == 0)
        {
            return null;
        }
        var i = UnityEngine.Random.Range(0, Deck.Count);
        var card = Deck[i];
        Deck.RemoveAt(i);
        Hands.Add(card);
        return card;
    }

    /// <summary>
    /// 回合开始初始化
    /// </summary>
    internal void Prepare()
    {
        Shuffle(DiscardPile);
        Deck.AddRange(DiscardPile);
        DiscardPile.Clear();
        DrawCard();
        DrawCard();
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    /// <param name="cards"></param>
    void Shuffle(List<Card> cards)
    {
        for(int i = 0; i < cards.Count; ++i)
        {
            int select = UnityEngine.Random.Range(0, cards.Count - i);
            var temp = cards[select];
            cards[select] = cards[cards.Count - i - 1];
            cards[cards.Count - i - 1] = temp;
        }
    }
}