using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameToolKit;
/// <summary>
/// ��Ƭ����
/// </summary>
public class TileSetting : ScriptableSingleton<TileSetting>
{
    public Dictionary<TileType, TileBase> TileDic = new();
    public TileBase MoveMaskTile;
    public TileBase DepolyMaskTile;
    public TileBase TargetMaskTile;
    public TileBase AttackMaskTile;
}
