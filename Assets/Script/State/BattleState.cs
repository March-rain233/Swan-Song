using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameToolKit;

/// <summary>
/// 战斗状态
/// </summary>
public class BattleState : GameState
{
    enum GameStatus
    {
        Victory,
        Continue,
        Failure
    }
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
    /// 开始摆放单位
    /// </summary>
    public event Action<List<Vector2Int>, List<UnitData>> DeployBeginning;
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
    /// 在场的玩家单位
    /// </summary>
    public IEnumerable<Player> PlayerList => UnitList.OfType<Player>();

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
    } = 0;

    public BattleAnimator Animator;
    public MapRenderer MapRenderer;
    public UnitRenderer UnitRenderer;
    protected internal override void OnEnter()
    {
        //初始化系统
        Map = MapFactory.CreateMap("");
        UnitList = EnemyFactory.CreateEnemy(Map, "");

        MapRenderer = new MapRenderer(this);
        UnitRenderer = new UnitRenderer(this);
        Animator = new BattleAnimator(this);

        //绘制地图与敌人
        MapRenderer.RenderMap(Map);
        foreach(var enemy in UnitList)
        {
            UnitRenderer.CreateUnitView(enemy);
        }

        ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel("DepolyPanel");

        //数据设置
        var depolyList = GetDeployPoints();
        var gm = ServiceFactory.Instance.GetService<GameManager>();
        var unitDatas = gm.GameData.Members;
        DeployBeginning?.Invoke(depolyList, unitDatas);
    }

    protected internal override void OnExit()
    {
        throw new NotImplementedException();
    }

    protected internal override void OnUpdata()
    {

    }

    /// <summary>
    /// 获取可部署的节点列表
    /// </summary>
    /// <returns></returns>
    List<Vector2Int> GetDeployPoints()
    {
        //todo
        return new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0) };
    }

    /// <summary>
    /// 部署单位
    /// </summary>
    /// <param name="depolyList"></param>
    public void DeployUnits(IEnumerable<(UnitData unit, Vector2Int point)> depolyList)
    {
        foreach(var pair in depolyList)
        {
            var unit = new Player(pair.unit, pair.point);
            UnitList.Add(unit);
            UnitRenderer.CreateUnitView(unit);
        }
        BeginBattle();
    }

    void BeginBattle()
    {
        var pm = ServiceFactory.Instance.GetService<PanelManager>();
        pm.ClosePanel("DepolyPanel");
        foreach(var unit in UnitList)
        {
            unit.Position = unit.Position;
        }
        pm.OpenPanel("BattlePanel");
        NextTurn();
    }

    /// <summary>
    /// 查找指定单位后一个行动的单位
    /// </summary>
    public Unit GetNextUnit(Unit unit)
    {
        return UnitList.Where(u=>u.ActionStatus == ActionStatus.Waitting)
            .OrderBy(u=>u.UnitData.Speed)
            .FirstOrDefault(u=>u != unit && u.UnitData.Speed >= unit.UnitData.Speed);
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
    /// 下一个单位行动
    /// </summary>
    void NextUnitTurn()
    {
        CurrentUnit = GetNextUnit(CurrentUnit);
        //当已无下一个单位执行时，当前回合结算
        if(CurrentUnit == null)
        {
            TurnEnding?.Invoke(RoundNumber);
            var status = CheckGame();
            switch (status)
            {
                case GameStatus.Victory:
                    //todo
                    break;
                case GameStatus.Continue:
                    NextTurn();
                    break;
                case GameStatus.Failure:
                    //todo
                    break;
            }
        }
        else
        {
            CurrentUnit.BeginTurn(NextUnitTurn);
        }
    }
    /// <summary>
    /// 下一回合
    /// </summary>
    void NextTurn()
    {
        RoundNumber += 1;
        foreach (var unit in UnitList)
        {
            unit.Prepare();
        }
        CurrentUnit = UnitList.OrderBy(u => u.UnitData.Speed).First();
        TurnBeginning?.Invoke(RoundNumber);
        CurrentUnit.BeginTurn(NextUnitTurn);
    }

    /// <summary>
    /// 检测胜负
    /// </summary>
    GameStatus CheckGame()
    {
        if(UnitList.Where(u=>u.Camp == Camp.Player && u.ActionStatus != ActionStatus.Dead).Count() <= 0)
        {
            return GameStatus.Failure;
        }
        else if(UnitList.Where(u=>u.Camp == Camp.Enemy && u.ActionStatus != ActionStatus.Dead).Count() <= 0)
        {
            return GameStatus.Victory;
        }
        return GameStatus.Continue;
    }
}