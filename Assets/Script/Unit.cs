using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <remarks>单位对象</remarks>
public abstract class Unit : IHurtable, ICurable
{
    /// <summary>
    /// 结束回合回调缓存
    /// </summary>
    private Action _callback;
    private List<Buff> _buffList;

    /// <summary>
    /// 当单位开始当前回合的行动
    /// </summary>
    public event Action TurnBeginning;
    /// <summary>
    /// 当单位结束当前回合的行动
    /// </summary>
    public event Action TurnEnding;
    /// <summary>
    /// 当单位进行当前回合的初始化
    /// </summary>
    public event Action Preparing;

    /// <summary>
    /// 该单位当前的行动状态
    /// </summary>
    public ActionStatus ActionStatus
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 卡牌调度器
    /// </summary>
    public CardScheduler Scheduler
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 该单位身上的Buff
    /// </summary>
    public IReadOnlyList<Buff> BuffList
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 单位的位置
    /// </summary>
    public UnityEngine.Vector2Int Position
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 单位数据
    /// </summary>
    public UnitData UnitData
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 行动点
    /// </summary>
    public int ActionPoint
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 回合开始初始化
    /// </summary>
    internal void Prepare()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 结束回合
    /// </summary>
    protected void EndTurn()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 开始回合
    /// </summary>
    internal void BeginTurn(Action callback)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 添加Buff
    /// </summary>
    public TBuff AddBuff<TBuff>() where TBuff : Buff
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 移除Buff
    /// </summary>
    public void RemoveBuff<TBuff>() where TBuff : Buff
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="target">移动目标位置</param>
    public void Move(Vector2Int target)
    {
        throw new System.NotImplementedException();
    }

    float IHurtable.HurtCalculate(float damage, HurtType type, object source)
    {
        throw new NotImplementedException();
    }

    void IHurtable.OnHurt(float damage)
    {
        throw new NotImplementedException();
    }

    float ICurable.CureCalculate(float power, object source)
    {
        throw new NotImplementedException();
    }

    void ICurable.OnCure(float power)
    {
        throw new NotImplementedException();
    }
}