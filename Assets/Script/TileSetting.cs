using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
/// <summary>
/// ��Ƭ����
/// </summary>
public class TileSetting : ScriptableSingleton<TileSetting>
{
    public Dictionary<int, UnityEngine.Tilemaps.TileBase> TileDic = new();
}
