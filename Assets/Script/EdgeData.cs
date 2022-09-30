using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 地图节点
/// </summary>
/// <remarks>用来处理寻路算法</remarks>
public class EdgeData
{
    /// <summary>
    /// 经过该边的消耗
    /// </summary>
    public int PrimaryCost;
    /// <summary>
    /// 目标节点的唯一标识符
    /// </summary>
    /// <remarks>请保证id和原节点能相互转化</remarks>
    public int ID;
    /// <summary>
    /// 经过该边的次级消耗
    /// </summary>
    /// <remarks>标识该路径的优劣，数值越高越不应该选择经过该边，用于辅助判断路径优先级</remarks>
    public int SecondaryCost;
}