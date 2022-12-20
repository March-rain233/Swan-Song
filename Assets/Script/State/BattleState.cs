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
    public enum ItemType
    {
        Money,
        Card,
        Artifact,
    }
    public class Item
    {
        public ItemType Type;
        public object Value;
    }
    enum GameStatus
    {
        Victory,
        Continue,
        Failure
    }
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

    /// <summary>
    /// 章节
    /// </summary>
    public int Chapter => GameManager.Instance.GameData.Chapter;
    /// <summary>
    /// 战斗难度等级
    /// </summary>
    public int BattleLevel { get; private set; }

    /// <summary>
    /// 战利品
    /// </summary>
    public List<Item> ItemList = new();

    #region 事件组
    public event Action<List<Item>> BootyIniting;
    /// <summary>
    /// 战斗初始化时
    /// </summary>
    public event Action BattleIniting;
    /// <summary>
    /// 战斗开始时
    /// </summary>
    public event Action BattleBeginning;
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
    #endregion

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

    public void InitSystem(int battleLevel)
    {
        BattleLevel = battleLevel;

        //初始化系统
        var mapData = MapFactory.CreateMap(battleLevel, Chapter);
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

        BattleIniting?.Invoke();

        foreach (var unit in UnitList)
        {
            unit.Init();
        }

        BattleBeginning?.Invoke();

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
    public void SelectCard(List<(Card, CardScheduler)> toSelect, Action<(Card, CardScheduler)> callback)
    {
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel("CardSelectPanel") as CardSelectPanel;
        panel.BtnBack.gameObject.SetActive(false);
        panel.SetCards(toSelect.Select(p=>(p.Item1, p.Item2.Unit.UnitData)));
        panel.CardSelected += (card, data) =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(panel);
            callback((card, toSelect.First(u=>u.Item2.Unit.UnitData == data).Item2));
        };
    }

    /// <summary>
    /// 下一个单位行动
    /// </summary>
    void NextUnitTurn()
    {
        CurrentUnit = GetNextUnit(CurrentUnit);
        CurrentUnitChanged?.Invoke(CurrentUnit);
        var status = CheckGame();
        switch (status)
        {
            case GameStatus.Continue:
                //当已无下一个单位执行时，当前回合结算
                if (CurrentUnit == null)
                {
                    TurnEnding?.Invoke(RoundNumber);
                    NextTurn();
                }
                else
                {
                    CurrentUnit.BeginTurn(NextUnitTurn);
                }
                break;
            case GameStatus.Victory:
                OnVictory();
                break;
            case GameStatus.Failure:
                OnFail();
                break;
        }
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
                (unit as IHurtable).Hurt(unit.UnitData.BloodMax * 0.2f, HurtType.FromBuff, this);
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

    void OnInitBooty()
    {
        var gm = GameManager.Instance;
        switch (BattleLevel)
        {
            case 1:
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        List<(Card, UnitData)> cards = new();
                        foreach (var member in gm.GameData.Members)
                        {
                            cards.Add((CardPoolManager.Instance.DrawCard(
                                (CardPoolManager.NormalPoolIndex, Card.CardRarity.Normal, 9),
                                (member.UnitModel.PrivilegeDeckIndex, Card.CardRarity.Privilege, 1))
                                , member));
                        }
                        ItemList.Add(new Item()
                        {
                            Type = ItemType.Card,
                            Value = cards
                        });
                    }
                    ItemList.Add(new Item()
                    {
                        Type = ItemType.Money,
                        Value = 30
                    });
                    break;
                }
            case 2:
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        List<(Card, UnitData)> cards = new();
                        foreach (var member in gm.GameData.Members)
                        {
                            cards.Add((CardPoolManager.Instance.DrawCard(
                                (CardPoolManager.NormalPoolIndex, Card.CardRarity.Normal, 7),
                                (member.UnitModel.PrivilegeDeckIndex, Card.CardRarity.Privilege, 3))
                                , member));
                        }
                        ItemList.Add(new Item()
                        {
                            Type = ItemType.Card,
                            Value = cards
                        });
                    }
                    ItemList.Add(new Item()
                    {
                        Type = ItemType.Money,
                        Value = 60
                    });
                    break;
                }
            case 3:
                {
                    foreach (var member in gm.GameData.Members)
                    {
                        for (int i = 0; i < 2; ++i)
                        {
                            List<(Card, UnitData)> cards = new();
                            for (int j = 0; j < 3; ++j)
                            {
                                cards.Add((CardPoolManager.Instance.DrawCard(
                                    (CardPoolManager.NormalPoolIndex, Card.CardRarity.Normal, 9),
                                    (member.UnitModel.PrivilegeDeckIndex, Card.CardRarity.Privilege, 1))
                                    , member));
                            }
                            ItemList.Add(new Item()
                            {
                                Type = ItemType.Card,
                                Value = cards
                            });
                        }
                    }
                    ItemList.Add(new Item()
                    {
                        Type = ItemType.Money,
                        Value = 120
                    });
                    var artifacts = ArtifactSetting.Instance.RemainArtifacts.ToList();
                    if (artifacts.Count > 0)
                    {
                        List<Artifact> list = new List<Artifact>();
                        for (int i = 0; i < 3 && artifacts.Count > 0; ++i)
                        {
                            var art = artifacts[UnityEngine.Random.Range(0, artifacts.Count)];
                            list.Add(art);
                        }
                        ItemList.Add(new Item() { Type = ItemType.Artifact, Value = list });
                    }
                    foreach(var m in GameManager.Instance.GameData.Members)
                    {
                        m.LevelUp();
                    }
                    break;
                }
        }

        BootyIniting?.Invoke(ItemList);
    }

    void OnVictory()
    {
        OnInitBooty();
        Successed?.Invoke();
    }

    void OnFail()
    {
        Failed?.Invoke();
    }
}