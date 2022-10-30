using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Í¼¿éµÄ¹éÊôÀàĞÍ
/// </summary>
[System.Flags]
public enum TileType
{
    Grass = 1,
    Lack = 1 << 1,
    Sand = 1 << 2,
    Mountain = 1 << 3,
}
