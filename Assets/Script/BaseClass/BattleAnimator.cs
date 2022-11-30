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

    BattlePanel _battlePanel => ServiceFactory.Instance.GetService<PanelManager>().GetOrOpenPanel("BattlePanel") as BattlePanel;

    Queue<Tween> _animQueue = new();
    public BattleAnimator(BattleState battleState)
    {
        var pm = ServiceFactory.Instance.GetService<PanelManager>();
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
        _battleState.Successed += () =>
        {
            var panel = pm.OpenPanel("SuccessPanel") as SuccessPanel;
            panel.ShowItems(_battleState.ItemList);
        };
        _battleState.Failed += () =>
        {
            pm.OpenPanel("FailurePanel");
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
        if(unit is Player)
        {
            unit.TurnBeginning += () =>
            {
                var anim = _battlePanel.BeginControl(unit as Player);
                EnqueueAnimation(anim);
            };
            unit.TurnEnding += () =>
            {
                var anim = _battlePanel.EndControl(unit as Player);
                EnqueueAnimation(anim);
            };
        }

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

            EnqueueAnimation(panel.RemoveCard(card, unit.Scheduler));
        };

        unit.UnitData.DataChanged += (data) =>
        {
            var copy = data.Clone();
            var anim = ExtensionDotween.GetEmptyTween(0.01f);
            anim.OnStart(()=>unitView.ViewData = copy);
            EnqueueAnimation(anim);
        };

        unit.BuffListChanged += () =>
        {
            var datas = unit.BuffList.Select(b => b.GetBuffData());
            var anim = ExtensionDotween.GetEmptyTween(0.01f);
            anim.OnStart(() => unitView.BuffDatas = datas);
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