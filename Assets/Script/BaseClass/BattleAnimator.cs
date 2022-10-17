using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using GameToolKit;
using DG.Tweening;

/// <summary>
/// 战场动画器
/// </summary>
public class BattleAnimator
{
    BattleState _battleState;
    MapRenderer _mapRenderer => _battleState.MapRenderer;
    Queue<Tween> _animQueue = new();
    public BattleAnimator(BattleState battleState)
    {
        _battleState = battleState;
        _battleState.CurrentUnitChanged += (unit) =>
        {
            List<Unit> orderList = _battleState.GetUnitOrderList().ToList();
            var panel = ServiceFactory.Instance.GetService<PanelManager>()
                .GetOrOpenPanel("BattlePanel") as BattlePanel;
            EnqueueAnimation(panel.NextUnit(orderList));
        };
        _battleState.TurnBeginning += (round) =>
        {
            var panel = ServiceFactory.Instance.GetService<PanelManager>()
                .GetOrOpenPanel("BattlePanel") as BattlePanel;
            EnqueueAnimation(panel.NextRound(round));
        };
    }

    /// <summary>
    /// 绑定单位动画
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="unitView"></param>
    public void BindindUnitAnimation(Unit unit, UnitView unitView)
    {
        unit.Moved += (rawPath) =>
        {
            var panel = ServiceFactory.Instance.GetService<PanelManager>().GetOrOpenPanel("BattlePanel") as BattlePanel;
            var cellWidth = _battleState.MapRenderer.Grid.cellSize.x;
            var path = rawPath.Select(p => _battleState.MapRenderer.Grid.CellToWorld(p.ToVector3Int()))
                .ToArray();
            Tween anim = unitView.transform.DOPath(path, 
                path.Length * 0.5f, 
                gizmoColor: Color.green);
            anim.SetEase(Ease.Linear);
            anim.OnWaypointChange(i =>
            {
                Vector2 dir = path[i + 1] - path[i];
                dir.Normalize();
                unitView.Animator.SetFloat("Direction_X", dir.x);
                unitView.Animator.SetFloat("Direction_Y", dir.y);
            });
            anim.OnPlay(() =>
            {
                if (unit is Player)
                {
                    panel.Disable();
                }
                unitView.Animator.Play("Move", 0, 0f);
            });
            anim.OnKill(()=>
            {
                if (unit is Player)
                {
                    panel.Enable();
                }
                unitView.Animator.Play("Idle", 0, 0f);
            });
            EnqueueAnimation(anim);
        };
        unit.Hurt += () =>
        {
            var anim = unitView.SpriteRenderer.DOColor(Color.red, 0.1f);
            anim.SetLoops(2, LoopType.Yoyo);
            EnqueueAnimation(anim);
        };
    }

    /// <summary>
    /// 压入动画
    /// </summary>
    /// <param name="tween"></param>
    void EnqueueAnimation(Tween tween)
    {
        tween.Pause();
        tween.OnComplete(() =>
        {
            _animQueue.Dequeue();
            Play();
        });
        _animQueue.Enqueue(tween);
        Play();
    }

    void Play()
    {
        if(_animQueue.TryPeek(out var next) && !next.IsPlaying())
        {
            next.Play();
        }
    }
}