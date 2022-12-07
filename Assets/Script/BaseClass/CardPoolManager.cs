using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.Serialization;

public class CardPoolManager : GameToolKit.ScriptableSingleton<CardPoolManager>
{
    [OdinSerialize]
    private List<Card> _pool;
    [OdinSerialize]
    private Dictionary<string, List<Card>> _poolDic = new();

    /// <summary>
    /// 抽取指定卡池中的一张卡
    /// </summary>
    /// <param name="poolIndex">卡池索引</param>
    public Card DrawCard(params string[] poolIndex)
    {
        return DrawCard(poolIndex.Select(p => (p, 1)).ToArray());
    }

    public Card DrawCard(params (string poolIndex, int weight)[] poolIndex)
    {
        int rand = AdvanceRandom.Draw(poolIndex
            .Select((p, i)=>(new AdvanceRandom.Item() { Value = i, Weight = p.weight})));
        var pool = _poolDic[poolIndex[rand].poolIndex];
        rand = UnityEngine.Random.Range(0, pool.Count);
        return pool[rand].Clone();
    }
}