using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct TileData
{
    public TileType TileType;
    public TileStatus TileStatus;

    /// <summary>
    /// 当前格上的单位
    /// </summary>
    public List<Unit> Units
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 进入该单元格
    /// </summary>
    public void Enter(Unit unit)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 离开该单元格
    /// </summary>
    public void Exit(Unit unit)
    {
        throw new System.NotImplementedException();
    }
}