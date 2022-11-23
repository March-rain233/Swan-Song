using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using GameToolKit;
using DG.Tweening;

/// <summary>
/// 战场动画器
/// </summary>
/// <remarks>
/// 用于管理战斗时视图层的逻辑
/// </remarks>
public class BattleAnimator
{
    public const float LongAnimationDuration = 0.5f;
    public const float MiddleAnimationDuration = 0.25f;
    public const float ShortAnimationDuration = 0.1f;

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
    /// 销毁服务
    /// </summary>
    public void Destroy()
    {

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

            //计算路线
            var anim = unitView.MoveAnim(rawPath);
            anim.onPlay += () =>
            {
                if (unit is Player)
                {
                    panel.Disable();
                }
            };
            anim.onKill += ()=>
            {
                if (unit is Player)
                {
                    panel.Enable();
                }
            };

            EnqueueAnimation(anim);
        };
        unit.Hurt += (_, _, _) =>
        {
            var anim = unitView.HurtAnim();
            EnqueueAnimation(anim);
        };
        unit.Scheduler.HandsAdded += (card) =>
        {
            var panel = ServiceFactory.Instance.GetService<PanelManager>().GetOrOpenPanel("BattlePanel") as BattlePanel;

            EnqueueAnimation(panel.AddCard(card, unit.Scheduler));
        };
        unit.Scheduler.HandsRemoved += (card) =>
        {
            var panel = ServiceFactory.Instance.GetService<PanelManager>().GetOrOpenPanel("BattlePanel") as BattlePanel;

            EnqueueAnimation(panel.RemoveCard(card));
        };

        unit.UnitData.DataChanged += () =>
        {
            var copy = unit.UnitData.Clone();
            var anim = ExtensionDotween.GetEmptyTween(0.01f);
            anim.OnStart(()=>unitView.ViewData = copy);
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