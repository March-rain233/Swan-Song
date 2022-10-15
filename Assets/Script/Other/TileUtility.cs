using GameToolKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class TileUtility
{
    /// <summary>
    /// 获取鼠标对应的格子
    /// </summary>
    /// <returns></returns>
    public static Vector3Int GetMouseInCell()
    {
        return ServiceFactory.Instance.GetService<MapRenderer>().Grid.WorldToCell(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    }

    /// <summary>
    /// 根据坐标尝试获取图格
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static bool TryGetTile(Vector2Int pos, out Tile tile)
    {
        var gm = ServiceFactory.Instance.GetService<GameManager>();
        var sta = gm.GetState() as BattleState;
        var map = sta.Map;
        tile = null;
        if(pos.x >=0 && pos.x < map.Width && pos.y >= 0 && pos.y < map.Height)
        {
            tile = map[pos.x, pos.y];
        }
        return tile != null;
    }
}
