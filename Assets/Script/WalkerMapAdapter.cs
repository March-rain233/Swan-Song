using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WalkerMapAdapter : MapAdapter
{
    public override List<EdgeData> GetAdjacency(int node, int cost = -1)
    {
        throw new NotImplementedException();
    }

    public override int ManhattonDistance(int start, int end)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 将坐标转化为唯一标识符
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int Point2ID(Vector2Int point)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 将唯一标识符转换为坐标
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Vector2Int ID2Point(int id)
    {
        throw new NotImplementedException();
    }
}