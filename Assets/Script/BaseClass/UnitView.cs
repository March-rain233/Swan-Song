using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.EventSystems;

/// <summary>
/// 单位视图
/// </summary>
public class UnitView : MonoBehaviour,IPointerClickHandler
{
    /// <summary>
    /// 该视图绑定的单位
    /// </summary>
    public Unit Unit
    {
        get => _unit;
        set
        {
            _unit = value;
            ViewData = _unit.UnitData.Clone();
            BuffDatas = _unit.BuffList.Select(b => b.GetBuffData());
        }
    }
    Unit _unit;

    /// <summary>
    /// 当前帧的展示单位数据
    /// </summary>
    public UnitData ViewData
    {
        get => _viewData;
        set
        {
            _viewData = value;
            ViewDataChanged?.Invoke(_viewData);
        }
    }
    UnitData _viewData;

    public IEnumerable<Buff.BuffData> BuffDatas
    {
        get => _buffDatas;
        set
        {
            _buffDatas = value;
            BuffChanged?.Invoke();
        }
    }
    IEnumerable<Buff.BuffData> _buffDatas;

    public Animator Animator;

    public SpriteRenderer SpriteRenderer;

    public Transform AttackTargetFlag;

    public event Action<UnitData> ViewDataChanged;
    public event Action BuffChanged;

    /// <summary>
    /// 是否被选定为目标
    /// </summary>
    public bool IsFlagged { get; private set; } = false;

    /// <summary>
    /// 当被点击时
    /// </summary>
    public event Action OnClicked;

    private void Awake()
    {
        AttackTargetFlag.gameObject.SetActive(false);
        IsFlagged = false;
        AttackTargetFlag.DOLocalMoveY(0.5f, 1f)
            .From(true)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        AttackTargetFlag
            .DOKill(true);
    }

    /// <summary>
    /// 标记为攻击对象
    /// </summary>
    public void Flag()
    {
        IsFlagged = true;
        AttackTargetFlag.gameObject.SetActive(true);
    }

    /// <summary>
    /// 取消标记
    /// </summary>
    public void Unflag()
    {
        IsFlagged = false;
        AttackTargetFlag.gameObject.SetActive(false);
    }

    public Tween HurtAnim()
    {
        return SpriteRenderer.DOColor(Color.red, 0.1f)
            .SetLoops(2, LoopType.Yoyo);
    }

    public Tween MoveAnim(IEnumerable<Vector2Int> rawPath)
    {
        var battleState = GameManager.Instance.GetState<BattleState>();
        var cellWidth = battleState.MapRenderer.Grid.cellSize.x;
        var path = rawPath.Select(p => battleState.MapRenderer.Grid.CellToWorld(p.ToVector3Int()))
            .ToArray();
        Tween anim = transform.DOPath(path,
            path.Length * 0.5f,
            gizmoColor: Color.green);
        anim.SetEase(Ease.Linear);
        anim.OnWaypointChange(i =>
        {
            Vector2 dir = path[i + 1] - path[i];
            dir.Normalize();
            Animator.SetFloat("Direction_X", dir.x);
            Animator.SetFloat("Direction_Y", dir.y);
        });
        anim.onPlay += () =>
        {
            Animator.Play("Move", 0, 0f);
        };
        anim.onKill += () =>
        {
            Animator.Play("Idle", 0, 0f);
        };
        return anim;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }
}
