using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 运行时的游戏地图数据
/// </summary>
public class Map : IEnumerable<(Vector2Int pos, Tile tile)>
{
    public Map(int width, int height)
    {
        _gridDatas = new Tile[width , height];
        _width = width;
        _height = height;
    }
    /// <summary>
    /// 地图数据
    /// </summary>
    private Tile[,] _gridDatas;

    public Tile this[int x, int y]
    {
        get
        {
            return _gridDatas[x, y];
        }
        set
        {
            _gridDatas[x, y] = value;
        }
    }
    public Tile this[Vector2Int pos]
    {
        get
        {
            return this[pos.x, pos.y];
        }
        set
        {
            this[pos.x, pos.y] = value;
        }
    }
    /// <summary>
    /// 宽度
    /// </summary>
    public int Width
    {
        get => _width;
        set
        {
            Resize(value, _height);
        }
    }
    int _width = 0;

    /// <summary>
    /// 高度
    /// </summary>
    public int Height
    {
        get => _height;
        set
        {
            Resize(_width, value);
        }
    }
    int _height = 0;

    public void Resize(int width, int height)
    {
        var datas = new Tile[width, height];
        int w, h;
        w = Math.Min(_width, width);
        h = Math.Min(_height, height);
        for(int i = 0; i < w; i++)
        {
            for(int j = 0; j < h; j++)
            {
                datas[i, j] = _gridDatas[i, j];
            }
        }
        _width = width;
        _height = height;
        _gridDatas = datas;
    }

    public void FillTile<TTile>(UnityEngine.RectInt rect)
        where TTile : Tile, new()
    {
        for(int i = rect.xMin; i < rect.xMax; ++i)
        {
            for(int j = rect.yMin; j < rect.yMax; ++j)
            {
                _gridDatas[i, j] = new TTile();
            }
        }
    }

    public void Updata()
    {
        foreach(var tile in _gridDatas)
        {
            tile.Updata();
        }
    }

    public IEnumerator<(Vector2Int pos, Tile tile)> GetEnumerator()
    {
        for(int i = 0; i < Width; ++i)
        {
            for(int j = 0; j < Height; ++j)
            {
                if(_gridDatas[i, j] != null)
                {
                    yield return (new Vector2Int(i, j), _gridDatas[i, j]);
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}