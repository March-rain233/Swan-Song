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
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 卡牌数据原型
    /// </summary>
    public CardModel PeototypeData
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 卡牌的行动费用
    /// </summary>
    public int Cost
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 是否已被强化
    /// </summary>
    public bool HasEnchanted
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 词条
    /// </summary>
    public HashSet<Entry> Entries
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 释放卡牌
    /// </summary>
    protected internal void Release()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 检查卡牌是否可以释放
    /// </summary>
    /// <param name="unit">持有卡牌的单位</param>
    protected internal bool CheckAvaliable(Unit unit)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 复制卡牌对象
    /// </summary>
    public Card Clone()
    {
        throw new System.NotImplementedException();
    }
}