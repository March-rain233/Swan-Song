using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 地点类型
/// </summary>
public enum PlaceType
{
    /// <summary>
    /// 普通战斗
    /// </summary>
    NormalBattle,
    /// <summary>
    /// 精英战斗
    /// </summary>
    AdvancedBattle,
    /// <summary>
    /// Boss战斗
    /// </summary>
    BossBattle,
    /// <summary>
    /// 起点
    /// </summary>
    Start,
    /// <summary>
    /// 营火
    /// </summary>
    BonFire
}