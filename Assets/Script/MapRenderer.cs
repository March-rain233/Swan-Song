using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameToolKit;

/// <summary>
/// 地图渲染器
/// </summary>
/// <remarks>根据地图数据设置图像效果</remarks>
public class MapRenderer:IService
{
    public Grid Grid { get; private set; }
    public Tilemap Tilemap { get; private set; }
    void IService.Init()
    {
        var grid = new GameObject("Grid",typeof(Grid));
        var tilemap = new GameObject("TileMap", typeof(Tilemap), typeof(TilemapRenderer));
        tilemap.transform.SetParent(grid.transform, false);
        Grid = grid.GetComponent<Grid>();
        Tilemap = tilemap.GetComponent<Tilemap>();
        Object.DontDestroyOnLoad(grid);
    }

    public void RenderMap(Map map)
    {
        Tilemap.ClearAllTiles();
        for(int i = 0;i < map.Width; ++i)
        {
            for(int j = 0;j < map.Height; ++j)
            {
                if(map[i,j] != null)
                {
                    Tilemap.SetTile(new Vector3Int(i, j, 0), TileSetting.Instance.TileDic[map[i, j].TileTypeID]);
                }
            }
        }
    }
}