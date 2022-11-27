using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using GameToolKit;
using UnityEngine.InputSystem;

/// <summary>
/// 手牌视图
/// </summary>
public class HandsView : SerializedMonoBehaviour
{
    public CanvasGroup CanvasGroup;

    public ArcLayout ArcLayout;
    public float DefaultMinnumAngle;
    public float DefaultPreferedAngle;
    public float FloatPreferedAngle;

    HandsCardView _select;
    /// <summary>
    /// 在被选中前的ui次序
    /// </summary>
    int _oriIndex;
    /// <summary>
    /// 可选目标
    /// </summary>
    Card.TargetData _avaliableTarget;

    /// <summary>
    /// 当前显示的玩家
    /// </summary>
    Player _player;

    bool _enable = false;

    /// <summary>
    /// 更改手牌显示角色
    /// </summary>
    /// <param name="player"></param>
    public Tween SwitchPlayer(Player player)
    {
        var seq = DOTween.Sequence(this);
        if (_player != player)
        {
            seq.AppendCallback(() =>
            {
                _player = player;
                _select = null;
                //将卡牌收拢
                foreach (HandsCardView ele in ArcLayout.Children)
                {
                    ele.PreferAngle = ele.MinnumAngle = ele.FlexibleAngle = 0;
                }
                ArcLayout.Refresh();
            });
            seq.AppendInterval(BattleAnimator.MiddleAnimationDuration);
            seq.AppendCallback(() =>
            {
                //新的卡牌展开
                foreach (HandsCardView ele in ArcLayout.Children)
                {
                    Destroy(ele.gameObject);
                }
                ArcLayout.Children.Clear();
                foreach(var card in player.Scheduler.Hands)
                {
                    AddCard(card, player.Scheduler);
                }
                ArcLayout.Refresh();
            });
            seq.AppendInterval(BattleAnimator.ShortAnimationDuration);
        }
        return seq;
    }

    /// <summary>
    /// 禁用
    /// </summary>
    public void Disable()
    {
        _enable = false;
        CanvasGroup.interactable = false;
        foreach (HandsCardView view in ArcLayout.Children)
        {
            view.Outline.enabled = false;
        }
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public void Enable()
    {
        _enable = true;
        CanvasGroup.interactable = true;
        Refresh();
    }

    public void Refresh()
    {
        foreach (HandsCardView view in ArcLayout.Children)
        {
            view.Outline.enabled = 
                _enable && view.CardScheduler.Unit.ActionStatus == ActionStatus.Running
                && view.Card.Cost <= view.CardScheduler.Unit.UnitData.ActionPoint;
        }
        ArcLayout.Refresh();
    }

    public void AddCard(Card card, CardScheduler scheduler)
    {
        if (scheduler == _player?.Scheduler)
        {
            //生成卡牌视图
            HandsCardView cardView = Instantiate(UISetting.Instance.PrefabsDic["HandsCardView"], transform)
                .GetComponent<HandsCardView>();

            //初始化卡牌
            cardView.Card = card;
            cardView.CardScheduler = scheduler;
            cardView.MinnumAngle = DefaultMinnumAngle;
            cardView.PreferAngle = DefaultPreferedAngle;
            cardView.MouseEntered += () =>
            {
                if (_select != null)
                {
                    return;
                }
                cardView.PreferAngle = FloatPreferedAngle;
                ArcLayout.Refresh();
            };
            cardView.MouseExited += () =>
            {
                if (_select != null)
                {
                    return;
                }
                cardView.PreferAngle = DefaultPreferedAngle;
                ArcLayout.Refresh();
            };
            cardView.MouseDown += () =>
            {
                if (_enable && _player.ActionStatus == ActionStatus.Running
                && cardView.Card.Cost <= cardView.CardScheduler.Unit.UnitData.ActionPoint)
                {
                    SelectCard(cardView);
                }
            };
            cardView.MouseUp += () =>
            {
                if (_enable && _player.ActionStatus == ActionStatus.Running
                && cardView.Card.Cost <= cardView.CardScheduler.Unit.UnitData.ActionPoint)
                {
                    UnSelect();
                }
            };
            cardView.MouseMove += () =>
            {
            };
            cardView.Refresh();

            cardView.Outline.enabled =
                _enable && cardView.CardScheduler.Unit.ActionStatus == ActionStatus.Running
                && cardView.Card.Cost <= cardView.CardScheduler.Unit.UnitData.ActionPoint;
            cardView.LineRenderer.enabled = false;

            //添加入布局器
            ArcLayout.Children.Add(cardView);
        }
    }

    /// <summary>
    /// 将卡牌移除手牌
    /// </summary>
    /// <param name="card"></param>
    public void DiscardCard(Card card, CardScheduler scheduler)
    {
        if (scheduler == _player?.Scheduler)
        {
            //移除卡牌视图
            var view = ArcLayout.Children.OfType<HandsCardView>().First(v => v.Card == card);
            var obj = view.gameObject;
            ArcLayout.Children.Remove(view);

            Destroy(view);

            //卡牌移除动画
            var anim = DOTween.Sequence();
            anim.Join(obj.transform.DOScale(Vector3.zero, BattleAnimator.ShortAnimationDuration));
            anim.Join(obj.transform.DOLocalMove(new Vector3(-500, 0), BattleAnimator.MiddleAnimationDuration));
            anim.onComplete = () => Destroy(obj);
        }
    }

    private void Update()
    {
        if(_select != null)
        {
            var map = GameManager.Instance.GetState<BattleState>().Map;
            var mr = GameManager.Instance.GetState<BattleState>().MapRenderer;
            var ur = GameManager.Instance.GetState<BattleState>().UnitRenderer;
            var mouse = TileUtility.GetMouseInCell().ToVector2Int();
            _select.SetLineEnd( _select.LineRenderer.transform.InverseTransformPoint(
                    ServiceFactory.Instance.GetService<PanelManager>().UICamera
                    .ScreenToWorldPoint(Mouse.current.position.ReadValue())));
            if (_avaliableTarget.AvaliableTile.Contains(mouse))
            {
                var list = _select.Card.GetAffecrTarget(_select.CardScheduler.Unit, mouse);
                mr.RenderAttackTile(list);
                ur.SelectUnit(list.Select(p => map[p.x, p.y])
                    .Where(t => t.Units.Count > 0)
                    .Select(t => t.Units.First()));
            }
            else
            {
                mr.RenderAttackTile();
                ur.SelectUnit();
            }
        }
    }

    void SelectCard(HandsCardView cardView)
    {
        _select = cardView;

        //将卡移入中央
        _select.transform.DOLocalMove(new Vector3(0, 50, 0), BattleAnimator.ShortAnimationDuration);
        _select.transform.DOLocalRotate(Vector3.zero, BattleAnimator.ShortAnimationDuration);
        _oriIndex = _select.transform.GetSiblingIndex();
        _select.transform.SetAsLastSibling();

        _select.LineRenderer.enabled = true;

        var mr = GameManager.Instance.GetState<BattleState>().MapRenderer;
        _avaliableTarget = _select.Card.GetAvaliableTarget(_select.CardScheduler.Unit);
        mr.RenderTargetTile(_avaliableTarget.ViewTiles);
    }

    void UnSelect()
    {
        var mr = GameManager.Instance.GetState<BattleState>().MapRenderer;
        var ur = GameManager.Instance.GetState<BattleState>().UnitRenderer;
        var mouse = TileUtility.GetMouseInCell().ToVector2Int();
        mr.RenderAttackTile();
        mr.RenderTargetTile();
        ur.SelectUnit();
        _select.LineRenderer.enabled = false;
        //当目标合法时释放卡牌，否则讲卡牌放回手牌
        if (_avaliableTarget.AvaliableTile.Contains(mouse))
        {
             _select.CardScheduler.ReleaseCard(_select.CardScheduler.Hands.IndexOf(_select.Card), mouse);
        }
        else
        {
            _select.transform.SetSiblingIndex(_oriIndex);
            _select.PreferAngle = DefaultPreferedAngle;
            ArcLayout.Refresh();
        }
        _select = null;
    }
}
