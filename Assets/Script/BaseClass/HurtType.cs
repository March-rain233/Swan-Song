using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 伤害类型
/// </summary>
[Flags]
public enum HurtType
{
    /// <summary>
    /// 来自单位的伤害
    /// </summary>
    FromUnit = 1,
    /// <summary>
    /// 来自Buff的伤害
    /// </summary>
    FromBuff = 1 << 1,
    /// <summary>
    /// 来自图块的伤害
    /// </summary>
    FromTile = 1 << 2,
    /// <summary>
    /// 近身伤害
    /// </summary>
    Melee = 1 << 3,
    /// <summary>
    /// 远程伤害
    /// </summary>
    Ranged = 1 << 4,
    /// <summary>
    /// 物理伤害
    /// </summary>
    AD = 1 << 5,
    /// <summary>
    /// 魔法伤害
    /// </summary>
    AP = 1 << 6,
    /// <summary>
    /// 即死
    /// </summary>
    Death = 1 << 7,
    /// <summary>
    /// 穿甲
    /// </summary>
    APDS = 1 << 8,
}