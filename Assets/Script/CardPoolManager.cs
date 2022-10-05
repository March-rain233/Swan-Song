using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CardPoolManager : GameToolKit.ScriptableSingleton<CardPoolManager>
{
    private Dictionary<string, CardPool> _poolDic = new Dictionary<string, CardPool>();

    /// <summary>
    /// 抽取指定卡池中的一张卡
    /// </summary>
    /// <param name="poolIndex">卡池索引</param>
    public CardModel DrawCard(params string[] poolIndex)
    {
        int rand = UnityEngine.Random.Range(0, poolIndex.Length);
        var pool = _poolDic[poolIndex[rand]];
        rand = UnityEngine.Random.Range(0, pool.Pool.Count);
        return pool.Pool[rand];
    }
}