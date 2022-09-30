using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Buff
{
    /// <summary>
    /// 生效的对象
    /// </summary>
    public Unit Unit
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 剩余回合数
    /// </summary>
    public int Rounds
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 该效果是否生效
    /// </summary>
    public bool IsEnable
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 使效果生效
    /// </summary>
    public void Enable()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 使效果失效
    /// </summary>
    public void Disable()
    {
        throw new System.NotImplementedException();
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();
}