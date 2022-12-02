using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Buff
{
    /// <summary>
    /// 用于展示的Buff数据
    /// </summary>
    public struct BuffData 
    {
        public string Name;
        public string Description;
        public UnityEngine.Sprite Sprite;
        public int Count;
    }

    /// <summary>
    /// 生效的对象
    /// </summary>
    public Unit Unit { get; private set; }

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

    internal void Register(Unit unit)
    {
        Unit = unit;
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    public virtual BuffData GetBuffData()
    {
        return new BuffData()
        {
            Count = Count,
        };
    }

    public virtual bool CheckReplace(Buff buff)
    {
        return buff.Count >= 0 && buff.Count < Count;
    }
}