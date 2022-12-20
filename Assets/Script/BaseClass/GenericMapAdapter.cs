using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GenericMapAdapter : MapAdapter
{
    Dictionary<int, Vector2Int> _IDDic = new();
    Map _map;
    Unit _unit;

    public GenericMapAdapter(Unit unit, Map map)
    {
        _map = map;
        _unit = unit;
    }

    public override IEnumerable<EdgeData> GetAdjacency(int node)
    {
        var center = _IDDic[node];
        List<EdgeData> edges = new();
        Action<Vector2Int> func = (Vector2Int offset) =>
        {
            var pos = center + offset;
            if(TileUtility.TryGetTile(pos, out var tile) && tile.CheckPlaceable(_unit))
            {
                //todo
                if(_unit.Camp == Camp.Player && _map[center].TileStatusList.FirstOrDefault(s => s is SilkscreenStatus) != null 
                && tile.TileStatusList.FirstOrDefault(s=>s is SilkscreenStatus) == null)
                {
                    return;
                }
                else if(tile.Units.Count > 0 && tile.Units.First().Camp != _unit.Camp)
                {
                    return;
                }
                edges.Add(new EdgeData() { ID = Point2ID(pos), PrimaryCost = 1 });
            }
        };
        func(Vector2Int.up);
        func(Vector2Int.down);
        func(Vector2Int.left);
        func(Vector2Int.right);
        return edges;
    }

    public override int ManhattonDistance(int start, int end)
    {
        var s = ID2Point(start);
        var e = ID2Point(end);
        return Mathf.Abs(s.x - e.x) + Mathf.Abs(s.y - e.y);
    }

    /// <summary>
    /// 将坐标转化为唯一标识符
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int Point2ID(Vector2Int point)
    {
        var id = point.y * _map.Width + point.x;
        if (_IDDic.TryGetValue(id, out var p) && p != point)
        {
            Debug.LogError($"hash collide new:{point} ori:{p}");
        }
        _IDDic[id] = point;
        return id;
    }

    /// <summary>
    /// 将唯一标识符转换为坐标
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Vector2Int ID2Point(int id)
    {
        return _IDDic[id];
    }
}