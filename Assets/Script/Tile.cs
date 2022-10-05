using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 地图块
/// </summary>
public abstract class Tile
{
    public TileStatus TileStatus;
    /// <summary>
    /// 图块种类ID
    /// </summary>
    public abstract int TileTypeID { get; }

    /// <summary>
    /// 当前格上的单位
    /// </summary>
    public IReadOnlyList<Unit> Units => _units;
    List<Unit> _units = new();

    /// <summary>
    /// 进入该单元格
    /// </summary>
    public void Enter(Unit unit)
    {
        _units.Add(unit);
        OnEnter(unit);
    }

    protected abstract void OnEnter(Unit unit);

    /// <summary>
    /// 离开该单元格
    /// </summary>
    public void Exit(Unit unit)
    {
        OnExit(unit);
        _units.Remove(unit);
    }

    protected abstract void OnExit(Unit unit);
}