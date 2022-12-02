﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 图块的叠加状态
/// </summary>
public abstract class TileStatus
{
    public Tile Tile { get; private set; }

    /// <summary>
    /// 剩余计数
    /// </summary>
    public int Count;

    /// <summary>
    /// 该效果是否生效
    /// </summary>
    public bool IsEnable
    {
        get;
        private set;
    }

    /// <summary>
    /// 使效果生效
    /// </summary>
    public void Enable()
    {
        IsEnable = true;
        OnEnable();
    }

    /// <summary>
    /// 使效果失效
    /// </summary>
    public void Disable()
    {
        IsEnable = false;
        OnDisable();
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    internal void Register(Tile tile)
    {
        Tile = tile;
    }

    /// <summary>
    /// 处理单位进入时的需要处理buff的图格状态
    /// </summary>
    internal protected abstract void StatusProcessOnEnter(IEnumerable<Unit> units);

    /// <summary>
    /// 处理单位退出时的需要处理uff的图格状态
    /// </summary>
    internal protected abstract void StatusProcessOnExit(IEnumerable<Unit> units);

    /// <summary>
    /// 处理单位处于图块上时的需要处理buff的图格状态
    /// </summary>
    internal protected abstract void StatusProcessOnUpdata(IEnumerable<Unit> units);
}

public abstract class RoundStatus : TileStatus
{
    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        if(Count > 0)
        {
            Count -= 1;
        }
        else if(Count == 0)
        {
            Tile.RemoveStatus(this);
        }
    }
}

public class FireStatus :RoundStatus
{
    protected override void OnDisable()
    {
        
    }

    protected override void OnEnable()
    {
        
    }

    protected internal override void StatusProcessOnEnter(IEnumerable<Unit> units)
    {
        foreach (var unit in units)
        {
            unit.AddBuff(new Burn() { Count = 2 });
        }
    }

    protected internal override void StatusProcessOnExit(IEnumerable<Unit> units)
    {
        
    }

    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        foreach (var unit in units)
        {
            unit.AddBuff(new Burn() { Count = 2 });
        }
    }
}

public class PoisonStatus : RoundStatus
{
    protected override void OnDisable()
    {

    }

    protected override void OnEnable()
    {

    }

    protected internal override void StatusProcessOnEnter(IEnumerable<Unit> units)
    {
        foreach (var unit in units)
        {
            unit.AddBuff(new Poison() { Count = 2 });
        }
    }

    protected internal override void StatusProcessOnExit(IEnumerable<Unit> units)
    {

    }

    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        base.StatusProcessOnUpdata(units);
        foreach (var unit in units)
        {
            unit.AddBuff(new Poison() { Count = 2 });
        }
    }
}