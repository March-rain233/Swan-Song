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
    FromUnit,
    /// <summary>
    /// 来自Buff的伤害
    /// </summary>
    FromBuff,
    /// <summary>
    /// 近身伤害
    /// </summary>
    Melee,
    /// <summary>
    /// 远程伤害
    /// </summary>
    Ranged,
    /// <summary>
    /// 物理伤害
    /// </summary>
    AD,
    /// <summary>
    /// 魔法伤害
    /// </summary>
    AP,
}