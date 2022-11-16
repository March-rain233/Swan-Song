using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

/// <summary>
/// 范围辅助器
/// </summary>
public class AreaHelper
{
    /// <summary>
    /// 标识
    /// </summary>
    public bool[,] Flags;
    /// <summary>
    /// 中心
    /// </summary>
    public Vector2Int Center;

    public List<Vector2Int> GetPointList(Vector2Int pos, Direction direction = Direction.Up)
    {
        var res = new List<Vector2Int>();
        var angle = Mathf.Atan2(direction.ToVector2Int().y, direction.ToVector2Int().x);
        Vector2Int offset = pos - Center; 
        for(int i = 0; i < Flags.GetLength(0); ++i)
        {
            for(int j = 0; j < Flags.GetLength(1); ++j)
            {
                if(Flags[i, j])
                {
                    res.Add(new Vector2Int(i * Mathf.Cos(a) + offset.x, j + offset.y));
                }
            }
        }
        return res;
    }
}
