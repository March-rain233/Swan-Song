using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class CardPoolManager : GameToolKit.ScriptableSingleton<CardPoolManager>
{
    [OdinSerialize, Searchable, FormerlySerializedAs("_pool")]
    public List<Card> TotalPool;
    [OdinSerialize, Searchable]
    public Dictionary<string, List<Card>> PoolDic{ get; private set;}

    public const string NormalPoolIndex = "Normal";

    /// <summary>
    /// 抽取指定卡池中的一张卡
    /// </summary>
    /// <param name="poolIndex">卡池索引</param>
    public Card DrawCard(string poolIndex, Card.CardRarity rarity)
    {
        return DrawCard((poolIndex, rarity, 1));
    }

    public Card DrawCard(params (string poolIndex, Card.CardRarity rarity, int weight)[] poolIndex)
    {
        int rand = AdvanceRandom.Draw(poolIndex
            .Select((p, i)=>(new AdvanceRandom.Item() { Value = i, Weight = p.weight})));
        var pool = PoolDic[poolIndex[rand].poolIndex].Where(c=>poolIndex[rand].rarity == c.Rarity).ToList();
        rand = UnityEngine.Random.Range(0, pool.Count());
        return pool.ElementAt(rand).CloneCard();
    }
}