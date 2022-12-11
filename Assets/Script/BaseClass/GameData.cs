using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 游戏数据
/// </summary>
/// <remarks>用于构建一局游戏所需的数据</remarks>
[Serializable]
public class GameData
{
    /// <summary>
    /// 该局游戏的随机数状态
    /// </summary>
    [JsonProperty]
    [JsonConverter(typeof(RandomStateConvert))]
    public UnityEngine.Random.State RandomState;
    /// <summary>
    /// 玩家持有的金币数量
    /// </summary>
    public int Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            GoldChanged?.Invoke();
        }
    }
    [SerializeField]
    int _gold;
    /// <summary>
    /// 队伍成员数据
    /// </summary>
    public List<UnitData> Members;

    /// <summary>
    /// 分支选择树
    /// </summary>
    public TreeMap TreeMap;

    /// <summary>
    /// 章节
    /// </summary>
    public int Chapter;

    public event Action GoldChanged;
}