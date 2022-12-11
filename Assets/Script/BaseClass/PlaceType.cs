using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

/// <summary>
/// 地点类型
/// </summary>
/// <summary>
/// 地点类型
/// </summary>
[Serializable]
public enum PlaceType
{
    /// <summary>
    /// 普通战斗
    /// </summary>
    [Description("遭遇战")]
    NormalBattle,
    /// <summary>
    /// 精英战斗
    /// </summary>
    [Description("精英战")]
    AdvancedBattle,
    /// <summary>
    /// Boss战斗
    /// </summary>
    [Description("头目战")]
    BossBattle,
    /// <summary>
    /// 起点
    /// </summary>
    [Description("起点")]
    Start,
    /// <summary>
    /// 营火
    /// </summary>
    [Description("营火")]
    BonFire,
    /// <summary>
    /// 占星台
    /// </summary>
    [Description("占星台")]
    PlatForm,
    /// <summary>
    /// 旅行商店
    /// </summary>
    [Description("旅行商店")]
    Store,
    /// <summary>
    /// 赌场
    /// </summary>
    [Description("赌场")]
    Casino,
    /// <summary>
    /// 路见不平
    /// </summary>
    [Description("路见不平")]
    FightForJus,
}