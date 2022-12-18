using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameToolKit;
/// <summary>
/// Õﬂ∆¨…Ë÷√
/// </summary>
public class TileSetting : ScriptableSingleton<TileSetting>
{
    public Dictionary<string, (int level, TileBase tile)> Tiles = new();
    public Dictionary<TileType, TileBase> TileDic = new();
    public TileBase MoveMaskTile;
    public TileBase DepolyMaskTile;
    public TileBase TargetMaskTile;
    public TileBase AttackMaskTile;
}
