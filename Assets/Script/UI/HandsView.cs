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
/// ������ͼ
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
    /// �ڱ�ѡ��ǰ��ui����
    /// </summary>
    int _oriIndex;
    /// <summary>
    /// ��ѡĿ��
    /// </summary>
    Card.TargetData _avaliableTarget;

    /// <summary>
    /// ����
    /// </summary>
    public void Disable()
    {
        CanvasGroup.interactable = false;
        foreach (HandsCardView view in ArcLayout.Children)
        {
            view.Outline.enabled = view.enabled = false;
        }
    }

    /// <summary>
    /// �ָ�
    /// </summary>
    public void Enable()
    {
        CanvasGroup.interactable = true;
        Refresh();
    }

    public void Refresh()
    {
        foreach (HandsCardView view in ArcLayout.Children)
        {
            view.Outline.enabled = view.enabled = view.CardScheduler.Unit.ActionStatus == ActionStatus.Running;
        }
    }

    public void AddCard(Card card, CardScheduler scheduler)
    {
        //���ɿ�����ͼ
        HandsCardView cardView = Instantiate(UISetting.Instance.PrefabsDic["HandsCardView"], transform)
            .GetComponent<HandsCardView>();

        //��ʼ������
        cardView.Card = card;
        cardView.CardScheduler = scheduler;
        cardView.MinnumAngle = DefaultMinnumAngle;
        cardView.PreferAngle = DefaultPreferedAngle;
        cardView.MouseEntered += () =>
        {
            if(_select != null)
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
            SelectCard(cardView);
        };
        cardView.MouseUp += () =>
        {
            UnSelect();
        };
        cardView.MouseMove += () =>
        {
        };
        cardView.Refresh();

        cardView.Outline.enabled =
            cardView.CardScheduler.Unit.ActionStatus == ActionStatus.Running;
        cardView.LineRenderer.enabled = false;

        //����벼����
        ArcLayout.Children.Add(cardView);
        ArcLayout.Refresh();
    }

    public void RemoveCard(Card card)
    {
        //�Ƴ�������ͼ
        var view = ArcLayout.Children.OfType<HandsCardView>().First(v=>v.Card == card);
        var obj = view.gameObject;
        ArcLayout.Children.Remove(view);
        ArcLayout.Refresh();

        Destroy(view);

        //�����Ƴ�����
        var anim = DOTween.Sequence();
        anim.Join(obj.transform.DOScale(Vector3.zero, BattleAnimator.ShortAnimationDuration));
        anim.Join(obj.transform.DOLocalMove(new Vector3(-500, 0), BattleAnimator.MiddleAnimationDuration));
        anim.onComplete = () => Destroy(obj);
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
        //��Ŀ��Ϸ�ʱ�ͷſ��ƣ����򽲿��ƷŻ�����
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
