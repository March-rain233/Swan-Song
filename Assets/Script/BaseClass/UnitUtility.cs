using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolKit.Utility;

public static class UnitUtility
{
    /// <summary>
    /// 寻找最短路径
    /// </summary>
    /// <param name="start">起点的id</param>
    /// <param name="end">终点的id</param>
    /// <param name="cost">路径的最高花费，为-1时为无限</param>
    /// <returns>路径节点的id</returns>
    public static IEnumerable<int> FindShortestPath(MapAdapter map, int start, int end, int cost = -1)
    {
        Dictionary<int, (int pre, int remain)> datas = new();
        Queue<int> queue = new Queue<int>();

        bool find = false;

        datas[start] = (start, cost);
        queue.Enqueue(start);
        while(queue.Count > 0 && !find)
        {
            int p = queue.Dequeue();
            var list = map.GetAdjacency(p);
            foreach (var e in list)
            {
                int remain = datas[p].remain - e.PrimaryCost;
                if (remain >= 0)
                {
                    if(datas.TryGetValue(e.ID, out var temp))
                    {
                        if(remain > temp.remain)
                        {
                            datas[e.ID] = (p, remain);
                            queue.Enqueue(e.ID);
                        }
                    }
                    else
                    {
                        datas[e.ID] = (p, remain);
                        queue.Enqueue(e.ID);
                    }
                    if (e.ID == end)
                    {
                        find = true;
                        break;
                    }
                }
            }
        }
        List<int> res = new();
        if (find)
        {
            int p = end;
            res.Add(p);
            while (p != start)
            {
                p = datas[p].pre;
                res.Add(p);
            }
            res.Reverse();
        }
        return res;
    }

    /// <summary>
    /// 查找全部可到达节点
    /// </summary>
    /// <param name="cost">最大花费</param>
    public static IEnumerable<int> GetAllAvailableNode(MapAdapter map, int start, int cost)
    {
        Dictionary<int, int> remains = new();
        Queue<int> queue = new Queue<int>();

        remains[start] = cost;
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            int p = queue.Dequeue();
            var list = map.GetAdjacency(p);
            foreach (var e in list)
            {
                int remain = remains[p] - e.PrimaryCost;
                if (remain >= 0)
                {
                    if (remains.TryGetValue(e.ID, out var temp))
                    {
                        if (remain > temp)
                        {
                            remains[e.ID] = remain;
                            queue.Enqueue(e.ID);
                        }
                    }
                    else
                    {
                        remains[e.ID] = remain;
                        queue.Enqueue(e.ID);
                    }
                }
            }
        }
        return remains.Keys;
    }
}