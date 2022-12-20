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
    public static MapData CreateMap(int battleLevel, int chapter)
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
        //获取敌方可放置点
        List<Vector2Int> posList = new List<Vector2Int>();
        for(int i = height / 3; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                var pos = new Vector2Int(j, i);
                if(data.Map[pos] != null && data.Map[pos] is not BarrierTile)
                {
                    posList.Add(pos);
                }
            }
        }
        //打乱列表
        for(int i = posList.Count - 1; i >= 0; i--)
        {
            var rand = Random.Range(0, i + 1);
            var temp = posList[rand];
            posList[rand] = posList[i];
            posList[i] = temp;
        }
        //创建单位
        data.Units = new List<Unit>();
        //创建boss
        if (battleLevel == 3)
        {

        }
        //创建精英
        if(battleLevel == 2)
        {
            int type = Random.Range(1, 2);
            Unit unit = type switch
            {
                1 => new MagicSpider(posList[0]),
                _ => throw new System.Exception("Over Monster Type"),
            };
            posList.RemoveAt(0);
            unit.Camp = Camp.Enemy;
            data.Units.Add(unit);
        }
        //创建小怪
        int num = Random.Range(chapter, chapter + 2);
        for(int i = 0; i < num; ++i)
        {
            int type = Random.Range(1, 12);
            Unit unit = type switch
            {
                1 => new Slime(posList[0]),
                2 => new Goblingunner(posList[0]),
                3 => new Goblinis(posList[0]),
                4 => new Skeletonarchers(posList[0]),
                5 => new Skeletalmage(posList[0]),
                6 => new BloodSuckFloater(posList[0]),
                7 => new ShaAttkMonster(posList[0]),
                8 => new UnstableSlime(posList[0]),
                9 => new GiantSkeleton(posList[0]),
                10 => new FungalSpider(posList[0]),
                11 => new SilkSpider(posList[0]),
                _ => throw new System.Exception("Over Monster Type"),
            };
            posList.RemoveAt(0);
            unit.Camp = Camp.Enemy;
            data.Units.Add(unit);
        }

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