using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 运行时的游戏地图数据
/// </summary>
public class Map
{
    /// <summary>
    /// 地图数据
    /// </summary>
    private List<TileData> _gridDatas;

    public TileData this[int x, int y]
    {
        get => default;
        set
        {

        }
    }

    /// <summary>
    /// 宽度
    /// </summary>
    public int Width
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 高度
    /// </summary>
    public int Height
    {
        get => default;
        set
        {
        }
    }
}