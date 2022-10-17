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
public class MapRenderer
{
    BattleState _battleState;
    public Grid Grid { get; private set; }

    Tilemap _tilemap;

    Tilemap _targetTileMask;
    Tilemap _moveMask;
    Tilemap _attackRangeMask;
    Tilemap _depolyMask;

    Dictionary<int, List<Vector2Int>> _maskLayers = new();

    public MapRenderer(BattleState battleState)
    {
        _battleState = battleState;
        var grid = new GameObject("Grid",typeof(Grid));
        Grid = grid.GetComponent<Grid>();
        _tilemap = CreateTileMap("Map");
        _moveMask = CreateTileMap("Mask", 0);
        _targetTileMask = CreateTileMap("Mask", 1);
        _attackRangeMask = CreateTileMap("Mask", 2);
        _depolyMask = CreateTileMap("Mask", 3);

        Grid.transform.localScale = new Vector3(2, 2, 1);
    }

    Tilemap CreateTileMap(string layer, int order = 0)
    {
        var obj = new GameObject($"TileMap({layer}-{order})", typeof(Tilemap), typeof(TilemapRenderer));
        var tilemap = obj.GetComponent<Tilemap>();
        var renderer = obj.GetComponent<TilemapRenderer>();
        renderer.sortingLayerName = layer;
        renderer.sortingOrder = order;
        obj.transform.SetParent(Grid.transform, false);
        return tilemap;
    }

    void RenderTilemap(Tilemap tilemap, TileBase tile,IEnumerable<Vector2Int> points = null)
    {
        tilemap.ClearAllTiles();
        if(points != null)
        {
            foreach (var point in points)
            {
                tilemap.SetTile(point.ToVector3Int(), tile);
            }
        }
    }

    /// <summary>
    /// 绘制移动范围
    /// </summary>
    /// <param name="points"></param>
    public void RenderMoveRange(IEnumerable<Vector2Int> points = null)
    {
        RenderTilemap(_moveMask, TileSetting.Instance.TileDic[0], points);
    }

    /// <summary>
    /// 绘制可作为释放目标的图块
    /// </summary>
    /// <param name="points"></param>
    public void RenderTargetTile(IEnumerable<Vector2Int> points = null)
    {
        RenderTilemap(_targetTileMask, TileSetting.Instance.TileDic[1], points);
    }

    /// <summary>
    /// 绘制AOE范围
    /// </summary>
    /// <param name="points"></param>
    public void RenderAttackRange(IEnumerable<Vector2Int> points = null)
    {
        RenderTilemap(_attackRangeMask, TileSetting.Instance.TileDic[2], points);
    }

    /// <summary>
    /// 绘制单位可放置格
    /// </summary>
    /// <param name="points"></param>
    public void RenderDepoly(IEnumerable<Vector2Int> points = null)
    {
        RenderTilemap(_depolyMask, TileSetting.Instance.TileDic[3], points);
    }

    /// <summary>
    /// 绘制地图
    /// </summary>
    /// <param name="map"></param>
    public void RenderMap(Map map)
    {
        _tilemap.ClearAllTiles();
        for(int i = 0;i < map.Width; ++i)
        {
            for(int j = 0;j < map.Height; ++j)
            {
                if(map[i,j] != null)
                {
                    _tilemap.SetTile(new Vector3Int(i, j, 0), TileSetting.Instance.TileDic[map[i, j].TileTypeID]);
                }
            }
        }
    }
}