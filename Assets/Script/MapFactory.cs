using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 地图工厂
/// </summary>
public static class MapFactory
{
    /// <summary>
    /// 根据描述创建地图
    /// </summary>
    /// <param name="description">地图生成描述</param>
    public static Map CreateMap(string description)
    {
        var map = new Map(20, 20);
        map.FillTile<GrassTile>(new UnityEngine.RectInt(0, 0, 20, 20));
        return map;
    }
}