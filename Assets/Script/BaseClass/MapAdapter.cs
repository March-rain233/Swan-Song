using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 地图适配器
/// </summary>
/// <remarks>用于提供给寻路算法</remarks>
public abstract class MapAdapter
{

    /// <summary>
    /// 获取邻接的可通行边
    /// </summary>
    /// <param name="node">目标节点</param>
    /// <param name="cost">最大花费，当值为-1时无视花费</param>
    public abstract IEnumerable<EdgeData> GetAdjacency(int node);

    /// <summary>
    /// 获得两个节点间的曼哈顿距离
    /// </summary>
    public abstract int ManhattonDistance(int start, int end);
}