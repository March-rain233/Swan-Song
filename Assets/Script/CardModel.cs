using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 卡牌数据模板
/// </summary>
public class CardModel
{
    /// <summary>
    /// 卡牌名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 卡牌简介
    /// </summary>
    public string Description;
    /// <summary>
    /// 卡面图片
    /// </summary>
    public Sprite Sprite;
    /// <summary>
    /// 卡牌的行动费用
    /// </summary>
    public int Cost;
    /// <summary>
    /// 原生携带词条
    /// </summary>
    public HashSet<Entry> NativeEntries;
}