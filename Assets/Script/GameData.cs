using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 游戏数据
/// </summary>
/// <remarks>用于构建一局游戏所需的数据</remarks>
public class GameData
{
    /// <summary>
    /// 该局游戏所使用的随机数种子
    /// </summary>
    public int Seed;
    /// <summary>
    /// 玩家持有的金币数量
    /// </summary>
    public int Gold;
    /// <summary>
    /// 队伍成员数据
    /// </summary>
    public List<UnitData> Members;

    /// <summary>
    /// 分支选择树
    /// </summary>
    public TreeMap TreeMap
    {
        get => default;
        set
        {
        }
    }
}