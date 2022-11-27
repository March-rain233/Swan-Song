using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 地图块
/// </summary>
public abstract class Tile
{
    public TileStatus TileStatus
    {
        get => _tileStatus;
        set
        {
            _tileStatus = value;
            TileStatusChanged?.Invoke(value);
            foreach(var unit in _units)
            {
                StatusProcessOnEnter(unit);
                StatusProcessOnUpdata(unit);
            }
        }
    }
    TileStatus _tileStatus;

    /// <summary>
    /// 图块种类I
    /// </summary>
    public TileType TileType;

    /// <summary>
    /// 当前格上的单位
    /// </summary>
    public IReadOnlyList<Unit> Units => _units;
    List<Unit> _units = new();

    public event Action<TileStatus> TileStatusChanged;

    /// <summary>
    /// 添加图格状态
    /// </summary>
    /// <param name="status"></param>
    public void AddStatus(TileStatus status)
    {
        if (!TileStatus.HasFlag(status))
        {
            //todo
            TileStatus = TileStatus | status;
        }
    }

    /// <summary>
    /// 进入该单元格
    /// </summary>
    public void Enter(Unit unit)
    {
        _units.Add(unit);
        OnEnter(unit);
        StatusProcessOnEnter(unit);
    }

    protected abstract void OnEnter(Unit unit);

    /// <summary>
    /// 离开该单元格
    /// </summary>
    public void Exit(Unit unit)
    {
        OnExit(unit);
        StatusProcessOnExit(unit);
        _units.Remove(unit);
    }

    protected abstract void OnExit(Unit unit);

    /// <summary>
    /// 检查指定单位是否可以被放置于该图格
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public virtual bool CheckPlaceable(Unit unit)
    {
        return true;
    }

    /// <summary>
    /// 处理单位进入时的需要处理buff的图格状态
    /// </summary>
    protected virtual void StatusProcessOnEnter(Unit unit)
    {
        if (TileStatus.HasFlag(TileStatus.Fire))
        {
            unit.AddBuff(new Burn() { Count = 2 });
        }
    }

    /// <summary>
    /// 处理单位退出时的需要处理uff的图格状态
    /// </summary>
    protected virtual void StatusProcessOnExit(Unit unit)
    {
    }

    /// <summary>
    /// 处理单位处于图块上时的需要处理buff的图格状态
    /// </summary>
    protected virtual void StatusProcessOnUpdata(Unit unit)
    {
        if (TileStatus.HasFlag(TileStatus.Fire))
        {
            unit.AddBuff(new Burn() { Count = 2 });
        }

    }

    internal void Updata()
    {
        foreach(var unit in _units)
        {
            StatusProcessOnUpdata(unit);
        }
    }
}
