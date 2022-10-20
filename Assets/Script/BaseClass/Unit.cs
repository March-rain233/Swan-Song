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
    private List<Buff> _buffList = new List<Buff>();

    /// <summary>
    /// 该单位当前的行动状态
    /// </summary>
    public ActionStatus ActionStatus
    {
        get;
        private set;
    } = ActionStatus.Waitting;

    /// <summary>
    /// 卡牌调度器
    /// </summary>
    public CardScheduler Scheduler
    {
        get;
        private set;
    }

    /// <summary>
    /// 该单位身上的Buff
    /// </summary>
    public IReadOnlyList<Buff> BuffList
    {
        get => _buffList;
    }

    /// <summary>
    /// 单位的位置
    /// </summary>
    public Vector2Int Position
    {
        get => _position;
        set
        {
            var manager = GameToolKit.ServiceFactory.Instance.GetService<GameManager>()
                .GetState() as BattleState;
            manager.Map[_position.x, _position.y].Exit(this);
            _position = value;
            manager.Map[_position.x, _position.y].Enter(this);
        }
    }
    Vector2Int _position = Vector2Int.zero;

    /// <summary>
    /// 单位数据
    /// </summary>
    public UnitData UnitData
    {
        get;
        private set;
    }

    /// <summary>
    /// 该单位所属阵营
    /// </summary>
    public Camp Camp;

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
    /// 单位移动事件
    /// </summary>
    public event Action<IEnumerable<Vector2Int>> Moved;
    /// <summary>
    /// 单位受伤事件
    /// </summary>
    public event Action Hurt;

    protected Unit(UnitData data, Vector2Int pos)
    {
        UnitData = data;
        Scheduler = new CardScheduler(this, data.Deck);
        _position = pos;
        UnitData.ActionPoint = UnitData.ActionPointMax;
    }

    /// <summary>
    /// 回合开始初始化
    /// </summary>
    internal void Prepare()
    {
        ActionStatus = ActionStatus.Waitting;
        if(UnitData.ActionPoint < UnitData.ActionPointMax)
        {
            UnitData.ActionPoint += 2;
        }
        Scheduler.Prepare();
        Preparing?.Invoke();
    }

    /// <summary>
    /// 结束回合
    /// </summary>
    protected void EndTurn()
    {
        if (ActionStatus != ActionStatus.Dead)
        {
            ActionStatus = ActionStatus.Rest;
        }
        TurnEnding?.Invoke();
        _callback();
    }

    /// <summary>
    /// 开始回合
    /// </summary>
    internal void BeginTurn(Action callback)
    {
        _callback = callback;
        ActionStatus = ActionStatus.Running;
        TurnBeginning?.Invoke();
        Decide();
    }

    /// <summary>
    /// 决策
    /// </summary>
    /// <remarks>
    /// 执行出牌逻辑
    /// </remarks>
    protected abstract void Decide();

    /// <summary>
    /// 添加Buff
    /// </summary>
    /// <remarks>
    /// 相同类型的buff将被替代
    /// </remarks>
    public void AddBuff(Buff buff)
    {
        var ori = _buffList.Find(e => e.GetType() == buff.GetType());
        if(ori != null)
        {
            ori.Disable();
            _buffList.Remove(ori);
        }
        _buffList.Add(buff);
        buff.Unit = this;
        buff.Enable();
    }

    /// <summary>
    /// 移除Buff
    /// </summary>
    public void RemoveBuff<TBuff>() where TBuff : Buff
    {
        var ori = _buffList.Find(e => e.GetType() == typeof(TBuff));
        if (ori != null)
        {
            ori.Disable();
            _buffList.Remove(ori);
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="target">移动目标位置</param>
    public void Move(Vector2Int target)
    {
        var adapter = new WalkerMapAdapter();
        var rawPath = UnitUtility.FindShortestPath(adapter, adapter.Point2ID(Position), adapter.Point2ID(target), UnitData.ActionPoint);
        var path = rawPath.Select(e => adapter.ID2Point(e));
        foreach(var p in path.Skip(1))
        {
            Position = p;
            //todo 行动点计算
            UnitData.ActionPoint -= 1;
        }
        Moved(path);
    }

    /// <summary>
    /// 获取可移动范围
    /// </summary>
    public IEnumerable<Vector2Int> GetMoveArea()
    {
        var adapter = new WalkerMapAdapter();
        var list = UnitUtility.GetAllAvailableNode(adapter, adapter.Point2ID(Position), UnitData.ActionPoint);
        return list.Select(e => adapter.ID2Point(e)).Where(e =>
        {
            return TileUtility.TryGetTile(e, out var tile) && tile.Units.Count == 0;
        });
    }

    float IHurtable.HurtCalculate(float damage, HurtType type, object source)
    {
        //todo:重新设计计算公式
        damage = Math.Max(0, damage - UnitData.Defence);
        damage = Math.Min(UnitData.Blood, damage);
        return damage;
    }

    void IHurtable.OnHurt(float damage)
    {
        UnitData.Blood -= (int)damage;
        Hurt?.Invoke();
        if(UnitData.Blood <= 0)
        {
            ActionStatus = ActionStatus.Dead;
            EndTurn();
        }
    }

    float ICurable.CureCalculate(float power, object source)
    {
        return power;
    }

    void ICurable.OnCure(float power)
    {
        UnitData.Blood += (int)power;
    }
}