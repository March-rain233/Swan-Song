using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 卡牌对象
/// </summary>
public abstract class Card
{
    /// <param name="model">模板</param>
    public Card(CardModel model)
    {
        PrototypeData = model;
    }

    /// <summary>
    /// 卡牌数据原型
    /// </summary>
    public CardModel PrototypeData
    {
        get;
        internal set;
    }

    /// <summary>
    /// 卡牌的行动费用
    /// </summary>
    public int Cost
    {
        get;
        set;
    }

    /// <summary>
    /// 是否已被强化
    /// </summary>
    public bool HasEnchanted
    {
        get;
        set;
    }

    /// <summary>
    /// 词条
    /// </summary>
    public HashSet<Entry> Entries
    {
        get;
        internal set;
    }

    /// <summary>
    /// 释放卡牌
    /// </summary>
    protected internal abstract void Release();

    /// <summary>
    /// 检查卡牌是否可以释放
    /// </summary>
    /// <param name="unit">持有卡牌的单位</param>
    protected internal abstract bool CheckAvaliable(Unit unit);

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