using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CardPoolManager : GameToolKit.ScriptableSingleton<CardPoolManager>
{
    private Dictionary<string, List<Card>> _poolDic = new();

    /// <summary>
    /// 抽取指定卡池中的一张卡
    /// </summary>
    /// <param name="poolIndex">卡池索引</param>
    public Card DrawCard(params string[] poolIndex)
    {
        int rand = UnityEngine.Random.Range(0, poolIndex.Length);
        var pool = _poolDic[poolIndex[rand]];
        rand = UnityEngine.Random.Range(0, pool.Count);
        return pool[rand].Clone();
    }
}