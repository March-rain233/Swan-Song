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
    public Tilemap MaskTilemap { get; private set; }

    public TileBase SquareTile => TileSetting.Instance.TileDic[0];
    void IService.Init()
    {
        var grid = new GameObject("Grid",typeof(Grid));
        Grid = grid.GetComponent<Grid>();
        Tilemap = CreateTileMap("Map");
        MaskTilemap = CreateTileMap("Mask");
        MaskTilemap.color = new Color(1, 1, 1, 0.8f);
        Object.DontDestroyOnLoad(grid);

        Grid.transform.localScale = new Vector3(2, 2, 1);
    }

    Tilemap CreateTileMap(string layer, int order = 0)
    {
        var obj = new GameObject($"TileMap({layer})", typeof(Tilemap), typeof(TilemapRenderer));
        var tilemap = obj.GetComponent<Tilemap>();
        var renderer = obj.GetComponent<TilemapRenderer>();
        renderer.sortingLayerName = layer;
        renderer.sortingOrder = order;
        obj.transform.SetParent(Grid.transform, false);
        return tilemap;
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