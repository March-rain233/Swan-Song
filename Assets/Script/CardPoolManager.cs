using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CardPoolManager
{
    private Dictionary<string, CardPool> _poolDic;

    /// <summary>
    /// 抽取指定卡池中的一张卡
    /// </summary>
    /// <param name="poolIndex">卡池索引</param>
    public CardModel DrawCard(params string[] poolIndex)
    {
        throw new System.NotImplementedException();
    }
}