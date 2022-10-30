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
    Normal,
    /// <summary>
    /// 火场
    /// </summary>
    Fire,
    /// <summary>
    /// 水流
    /// </summary>
    Water,
    /// <summary>
    /// 毒雾
    /// </summary>
    Poison,
    /// <summary>
    /// 浓雾
    /// </summary>
    Smog
}