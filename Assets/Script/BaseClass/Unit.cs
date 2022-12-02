using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameToolKit;

/// <remarks>单位对象</remarks>
public abstract class Unit : IHurtable, ICurable
{
    /// <summary>
    /// 结束回合回调缓存
    /// </summary>
    private Action _callback;
    private List<Buff> _buffList = new List<Buff>();
    public Direction Direction;

    /// <summary>
    /// 该单位当前的行动状态
    /// </summary>
    public ActionStatus ActionStatus
    {
        get => _actionStatus;
        set
        {
            _actionStatus = value;
            if(_actionStatus == ActionStatus.Dead)
            {
                Died();
            }
        }
    }
    ActionStatus _actionStatus = ActionStatus.Waitting;

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
    /// 是否可移动
    /// </summary>
    public bool CanMove = true;

    #region 事件组
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
    public event Action<float, HurtType, object> Hurt;
    /// <summary>
    /// 单位受伤计算事件
    /// </summary>
    public event Action<HurtCalculateEvent> HurtCalculating;
    /// <summary>
    /// 单位死亡事件
    /// </summary>
    public event Action UnitDied;

    /// <summary>
    /// Buff列表更改事件
    /// </summary>
    public event Action BuffListChanged;
    #endregion

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
        if(UnitData.ActionPoint < UnitData.ActionPointMax + 2)
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
        if (ActionStatus == ActionStatus.Running)
        {
            Decide();
        }
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
    public void AddBuff<TBuff>(TBuff buff) 
        where TBuff : Buff
    {
        foreach(var ori in _buffList.Where(e=>e is TBuff))
        {
            if (buff.CheckReplace(ori))
            {
                ori.Disable();
                _buffList.Remove(ori);
            }
        }
        _buffList.Add(buff);
        buff.Register(this);
        buff.Enable();

        BuffListChanged?.Invoke();
    }

    /// <summary>
    /// 移除Buff
    /// </summary>
    public void RemoveBuff<TBuff>() 
        where TBuff : Buff
    {
        var ori = _buffList.Find(e => e.GetType() == typeof(TBuff));
        if (ori != null)
        {
            ori.Disable();
            _buffList.Remove(ori);

            BuffListChanged?.Invoke();
        }
    }

    public void RemoveBuff(Buff buff)
    {
        buff.Disable();
        _buffList.Remove(buff);

        BuffListChanged?.Invoke();
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="target">移动目标位置</param>
    public void Move(Vector2Int target)
    {
        var adapter = new GenericMapAdapter(this, GameManager.Instance.GetState<BattleState>().Map);
        var rawPath = UnitUtility.FindShortestPath(adapter, adapter.Point2ID(Position), adapter.Point2ID(target), UnitData.ActionPoint);
        var path = rawPath.Select(e => adapter.ID2Point(e));
        var pre = path.First();
        foreach(var p in path.Skip(1))
        {
            Position = p;
            Direction = (Position - pre).ToDirection();
            pre = p;
            //todo 行动点计算
            UnitData.ActionPoint -= 1;
        }
        Moved(path);
    }

    void Died()
    {
        OnDied();
        UnitDied?.Invoke();
    }

    /// <summary>
    /// 当死亡时
    /// </summary>
    protected virtual void OnDied()
    {

    }

    /// <summary>
    /// 获取可移动范围
    /// </summary>
    public IEnumerable<Vector2Int> GetMoveArea()
    {
        if (!CanMove)
        {
            return Enumerable.Empty<Vector2Int>();
        }
        var adapter = new GenericMapAdapter(this, GameManager.Instance.GetState<BattleState>().Map);
        var list = UnitUtility.GetAllAvailableNode(adapter, adapter.Point2ID(Position), UnitData.ActionPoint);
        return list.Select(e => adapter.ID2Point(e)).Where(e =>
        {
            return TileUtility.TryGetTile(e, out var tile) && tile.Units.Count == 0;
        });
    }

    float IHurtable.HurtCalculate(float damage, HurtType type, object source)
    {
        //todo:重新设计计算公式
        HurtCalculateEvent @event = new HurtCalculateEvent()
        {
            OriDamage = damage,
            DamageAdd = -UnitData.Defence,
            Rate = 1,
            Source = source,
            Target = this,
            Type = type,
        };
        HurtCalculating?.Invoke(@event);
        ServiceFactory.Instance.GetService<EventManager>()
            .Broadcast(@event);
        damage = Math.Max(0, @event.FinalDamage);
        damage = Math.Min(UnitData.Blood, damage);
        return damage;
    }

    void IHurtable.OnHurt(float damage, HurtType type, object source)
    {
        UnitData.Blood -= (int)damage;
        Hurt?.Invoke(damage, type, source);
        if(UnitData.Blood <= 0)
        {
            if (ActionStatus == ActionStatus.Running)
            {
                ActionStatus = ActionStatus.Dead;
                EndTurn();
            }
            else
            {
                ActionStatus = ActionStatus.Dead;
            }
        }
    }

    float ICurable.CureCalculate(float power, object source)
    {
        return power;
    }

    void ICurable.OnCure(float power)
    {
        UnitData.Blood = Mathf.Max(UnitData.BloodMax, UnitData.Blood + (int)power);
    }
}