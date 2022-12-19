using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 地图工厂
/// </summary>
public static class MapFactory
{
    public struct MapData
    {
        public Map Map;
        public List<Unit> Units;
        public List<Vector2Int> PlaceablePoints;
    }

    public static float[,] GenerateNoiseMap(int width, int height, float frequence,float scale, Vector2 offset)
    {
        var map = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                map[i, j] = Mathf.PerlinNoise(i / (float)width * frequence + offset.x, j / (float)height * frequence + offset.y) * scale;
            }
        }
        return map;
    }

    /// <summary>
    /// 根据描述创建地图
    /// </summary>
    /// <param name="description">地图生成描述</param>
    public static MapData CreateMap(string description)
    {
        var data = new MapData();

        //创建地图
        int width, height;
        width = 8;
        height = 8;
        var heightMap = GenerateNoiseMap(width, height, 1.5f, 10, new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
        data.Map = new Map(width, height);
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Tile tile;
                float h = heightMap[i, j];
                if (h <= 4)
                {
                    tile = new BarrierTile() { TileType = TileType.Lack };
                }
                else
                {
                    tile = new NormalTile() { TileType = TileType.Grass };
                }
                data.Map[i, j] = tile;
            }
        }
        //创建单位
        data.Units = new() { new SilkSpider(new Vector2Int(5, 5)) { Camp = Camp.Enemy} };

        //设定可放置节点
        data.PlaceablePoints = new();
        for(int i = 0; i < height / 3; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if(data.Map[j, i] is NormalTile)
                {
                    data.PlaceablePoints.Add(new Vector2Int(j, i));
                }
            }
        }
        return data;
    }
}