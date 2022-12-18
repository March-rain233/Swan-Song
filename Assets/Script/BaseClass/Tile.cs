using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 地图块
/// </summary>
public abstract class Tile
{
    public IReadOnlyList<TileStatus> TileStatusList
    {
        get => _statusList;
    }
    List<TileStatus> _statusList = new List<TileStatus>();

    /// <summary>
    /// 图块种类I
    /// </summary>
    public TileType TileType;

    /// <summary>
    /// 当前格上的单位
    /// </summary>
    public IReadOnlyList<Unit> Units => _units;
    List<Unit> _units = new();

    public event Action TileStatusChanged;
    public event Action<TileStatus> TileStatusAdded;
    public event Action<TileStatus> TileStatusRemoved;

    /// <summary>
    /// 添加图格状态
    /// </summary>
    /// <param name="status"></param>
    public void AddStatus<TStatus>(TStatus status)
        where TStatus : TileStatus
    {
        foreach(var ori in _statusList.Where(e=>e is TStatus).ToList())
        {
            OnRemoveStatus(ori);
        }
        _statusList.Add(status);
        status.Register(this);
        status.Enable();
        status.StatusProcessOnEnter(_units);
        status.StatusProcessOnUpdata(_units);
        TileStatusAdded?.Invoke(status);
        TileStatusChanged?.Invoke();
    }

    void OnRemoveStatus(TileStatus status)
    {
        status.StatusProcessOnExit(_units);
        status.Disable();
        _statusList.Remove(status);
        TileStatusRemoved?.Invoke(status);
        TileStatusChanged?.Invoke();
    }

    public void RemoveStatus(TileStatus tileStatus)
    {
        OnRemoveStatus(tileStatus);
    }
    public void RemoveStatus<TStatus>()
        where TStatus : TileStatus
    {
        var ori = _statusList.Find(e => e is TStatus);
        if (ori != null)
        {
            OnRemoveStatus(ori);
        }
    }

    /// <summary>
    /// 进入该单元格
    /// </summary>
    public void Enter(Unit unit)
    {
        _units.Add(unit);
        OnEnter(unit);
        StatusProcessOnEnter();
    }

    protected abstract void OnEnter(Unit unit);

    /// <summary>
    /// 离开该单元格
    /// </summary>
    public void Exit(Unit unit)
    {
        OnExit(unit);
        StatusProcessOnExit();
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
    protected void StatusProcessOnEnter()
    {
        foreach(var status in _statusList)
        {
            status.StatusProcessOnEnter(_units);
        }
    }

    /// <summary>
    /// 处理单位退出时的需要处理uff的图格状态
    /// </summary>
    protected void StatusProcessOnExit()
    {
        foreach (var status in _statusList)
        {
            status.StatusProcessOnExit(_units);
        }
    }

    /// <summary>
    /// 处理单位处于图块上时的需要处理buff的图格状态
    /// </summary>
    protected void StatusProcessOnUpdata()
    {
        for(int i = _statusList.Count -1; i>= 0; i--)
        {
            _statusList[i].StatusProcessOnUpdata(_units);
        }
    }

    internal void Updata()
    {
        StatusProcessOnUpdata();
    }
}
