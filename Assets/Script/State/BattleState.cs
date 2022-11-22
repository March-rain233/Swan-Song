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
    /// 当当前决策单位变化
    /// </summary>
    public event Action<Unit> CurrentUnitChanged;
    /// <summary>
    /// 开始摆放单位
    /// </summary>
    public event Action<List<Vector2Int>, List<UnitData>> DeployBeginning;
    /// <summary>
    /// 胜利
    /// </summary>
    public event Action Successed;
    /// <summary>
    /// 失败
    /// </summary>
    public event Action Failed;
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

    /// <summary>
    /// 灭歌开始回合
    /// </summary>
    public int SwanSongRoundNumber;

    public BattleAnimator Animator;
    public MapRenderer MapRenderer;
    public UnitRenderer UnitRenderer;
    protected internal override void OnEnter()
    {
    }

    protected internal override void OnExit()
    {
        Animator.Destroy();
        MapRenderer.Destroy();
        UnitRenderer.Destroy();
        ServiceFactory.Instance.GetService<PanelManager>()
            .ClosePanel("BattlePanel");
    }

    protected internal override void OnUpdata()
    {

    }

    public void InitSystem()
    {
        //初始化系统
        var mapData = MapFactory.CreateMap("");
        Map = mapData.Map;
        UnitList = mapData.Units;
        //todo:设置灭歌开始回合
        SwanSongRoundNumber = 20;

        MapRenderer = new MapRenderer(this);
        UnitRenderer = new UnitRenderer(this);
        Animator = new BattleAnimator(this);

        //绘制地图与敌人
        MapRenderer.RenderMap(Map);
        foreach (var enemy in UnitList)
        {
            UnitRenderer.CreateUnitView(enemy);
        }

        ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel("DepolyPanel");

        //数据设置
        var depolyList = mapData.PlaceablePoints;
        var gm = ServiceFactory.Instance.GetService<GameManager>();
        var unitDatas = gm.GameData.Members;
        DeployBeginning?.Invoke(depolyList, unitDatas);
    }

    /// <summary>
    /// 部署单位
    /// </summary>
    /// <param name="depolyList"></param>
    public void DeployUnits(IEnumerable<(UnitData unit, Vector2Int point)> depolyList)
    {
        foreach (var pair in depolyList)
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
        foreach (var unit in UnitList)
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
        return UnitList.Where(u => u.ActionStatus == ActionStatus.Waitting)
            .OrderByDescending(u => u.UnitData.Speed)
            .FirstOrDefault(u => unit == null || (u != unit && u.UnitData.Speed <= unit.UnitData.Speed));
    }

    /// <summary>
    /// 获取当前的单位次序列表
    /// </summary>
    public IEnumerable<Unit> GetUnitOrderList()
    {
        return UnitList.Where(u => u.ActionStatus == ActionStatus.Waitting || u.ActionStatus == ActionStatus.Running)
            .OrderByDescending(u => u.UnitData.Speed);
    }

    /// <summary>
    /// 让玩家从待选列表中选择一张卡
    /// </summary>
    /// <param name="toSelect"></param>
    /// <param name="callback"></param>
    public void SelectCard(List<Card> toSelect, Action<Card> callback)
    {
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel("CardSelectPanel") as CardSelectPanel;
        panel.SetCards(toSelect);
        panel.OnCardSelected += (card) =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(panel);
            callback(card);
        };
    }

    /// <summary>
    /// 下一个单位行动
    /// </summary>
    void NextUnitTurn()
    {
        CurrentUnit = GetNextUnit(CurrentUnit);
        //当已无下一个单位执行时，当前回合结算
        if (CurrentUnit == null)
        {
            TurnEnding?.Invoke(RoundNumber);
            var status = CheckGame();
            switch (status)
            {
                case GameStatus.Victory:
                    //todo
                    Successed?.Invoke();
                    break;
                case GameStatus.Continue:
                    NextTurn();
                    break;
                case GameStatus.Failure:
                    //todo
                    Failed?.Invoke();
                    break;
            }
        }
        else
        {
            CurrentUnit.BeginTurn(NextUnitTurn);
        }
        CurrentUnitChanged?.Invoke(CurrentUnit);
    }
    /// <summary>
    /// 下一回合
    /// </summary>
    void NextTurn()
    {
        RoundNumber += 1;

        //发动灭亡之歌
        if (RoundNumber >= SwanSongRoundNumber)
        {
            foreach (var unit in UnitList.Where(u => u.ActionStatus != ActionStatus.Dead))
            {
                (unit as IHurtable).Hurt(unit.UnitData.BloodMax * 0.35f, HurtType.FromBuff, this);
            }
        }

        Map.Updata();

        foreach (var unit in UnitList.Where(u => u.ActionStatus != ActionStatus.Dead))
        {
            unit.Prepare();
        }
        CurrentUnit = null;
        TurnBeginning?.Invoke(RoundNumber);
        NextUnitTurn();
    }

    /// <summary>
    /// 检测胜负
    /// </summary>
    GameStatus CheckGame()
    {
        if (UnitList.Where(u => u.Camp == Camp.Player && u.ActionStatus != ActionStatus.Dead).Count() <= 0)
        {
            return GameStatus.Failure;
        }
        else if (UnitList.Where(u => u.Camp == Camp.Enemy && u.ActionStatus != ActionStatus.Dead).Count() <= 0)
        {
            return GameStatus.Victory;
        }
        return GameStatus.Continue;
    }
}