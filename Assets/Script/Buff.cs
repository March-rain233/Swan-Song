using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Buff
{
    /// <summary>
    /// 生效的对象
    /// </summary>
    public Unit Unit;

    /// <summary>
    /// 剩余回合数
    /// </summary>
    public int Rounds;

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
}