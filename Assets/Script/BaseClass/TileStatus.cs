using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 图块的叠加状态
/// </summary>
[System.Flags]
public enum TileStatus
{
    /// <summary>
    /// 常态
    /// </summary>
    Normal = 0,
    /// <summary>
    /// 火场
    /// </summary>
    Fire = 1,
    /// <summary>
    /// 水流
    /// </summary>
    Water = 1 << 1,
    /// <summary>
    /// 毒雾
    /// </summary>
    Poison = 1 << 2,
    /// <summary>
    /// 浓雾
    /// </summary>
    Smog = 1 << 3,
}