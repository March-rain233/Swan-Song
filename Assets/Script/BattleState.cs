using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 战斗状态
/// </summary>
public class BattleState : GameState
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
        get;
        private set;
    }

    /// <summary>
    /// 当前在场的单位
    /// </summary>
    public List<Unit> UnitList
    {
        get;
        private set;
    } = new();

    /// <summary>
    /// 当前进行操作的单位
    /// </summary>
    public Unit CurrentUnit
    {
        get;
        private set;
    }

    /// <summary>
    /// 当前的回合数
    /// </summary>
    /// <remarks>从1开始</remarks>
    public int RoundNumber
    {
        get;
        private set;
    } = 1;

    protected internal override void OnEnter()
    {
        Map = MapFactory.CreateMap("");
        UnitList = EnemyFactory.CreateEnemy(Map, "");
        GameToolKit.ServiceFactory.Instance.GetService<MapRenderer>()
            .RenderMap(Map);
    }

    protected internal override void OnExit()
    {
        throw new NotImplementedException();
    }

    protected internal override void OnUpdata()
    {

    }

    /// <summary>
    /// 查找指定单位后一个行动的单位
    /// </summary>
    public Unit GetNextUnit(Unit unit)
    {
        return UnitList.Where(u=>u.ActionStatus == ActionStatus.Waitting)
            .OrderBy(u=>u.UnitData.Speed)
            .First(u=>u != unit && u.UnitData.Speed >= unit.UnitData.Speed);
    }

    /// <summary>
    /// 获取当前的单位次序列表
    /// </summary>
    public IEnumerable<Unit> GetUnitOrderList()
    {
        return UnitList.Where(u => u.ActionStatus == ActionStatus.Waitting || u.ActionStatus == ActionStatus.Running)
            .OrderBy(u => u.UnitData.Speed);
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