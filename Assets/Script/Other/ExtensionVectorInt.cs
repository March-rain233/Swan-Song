using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionVectorInt
{
    public static Vector3Int ToVector3Int(this Vector2Int vector2Int)
    {
        return new Vector3Int(vector2Int.x, vector2Int.y, 0);
    }

    public static Vector2Int ToVector2Int(this Vector3Int vector3Int)
    {
        return new Vector2Int(vector3Int.x, vector3Int.y);
    }
}
