using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 战斗状态
/// </summary>
public class BattleStatus : GameStatus
{
    /// <summary>
    /// 回合开始时
    /// </summary>
    /// <remarks>传入当前回合数</remarks>
    public event Action<int> TurnBeginning;
    /// <summary>
    /// 回合结束时
    /// </summary>
    /// <remarks>传入当前回合数</remarks>
    public event Action<int> TurnEnding;

    /// <summary>
    /// 当前地图
    /// </summary>
    public Map Map
    {
        get => default;
        private set
        {
        }
    }

    /// <summary>
    /// 当前在场的单位
    /// </summary>
    public List<Unit> UnitList
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 当前进行操作的单位
    /// </summary>
    public Unit CurrentUnit
    {
        get => default;
    }

    /// <summary>
    /// 当前的回合数
    /// </summary>
    /// <remarks>从1开始</remarks>
    public int RoundNumber
    {
        get => default;
        set
        {
        }
    }

    protected internal override void OnEnter()
    {
        throw new NotImplementedException();
    }

    protected internal override void OnExit()
    {
        throw new NotImplementedException();
    }

    protected internal override void OnUpdata()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 查找指定单位后一个行动的单位
    /// </summary>
    public Unit GetNextUnit(Unit unit)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 获取当前的单位次序列表
    /// </summary>
    public List<Unit> GetUnitOrderList()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    /// <remarks>当玩家摆放完单位后调用</remarks>
    public void BeginBattle()
    {
        throw new System.NotImplementedException();
    }
}