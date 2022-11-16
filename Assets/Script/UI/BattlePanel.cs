using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using GameToolKit;
using DG.Tweening;
public class BattlePanel : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.Normal;

    #region UI控件
    public HandsView Hands;
    public UnitDataView UnitDataView;

    public TextMeshProUGUI AP;
    public Button BtnFinish;
    public Button BtnDiscard;
    public Button BtnDeck;
    public Toggle TogMove;

    public TextMeshProUGUI RoundNumber;
    public TextMeshProUGUI SwanSongRoundNumber;

    public CardListView Discard;
    public Button BtnDiscardReturn;

    public CardListView Deck;
    public Button BtnDeckReturn;

    public HorizontalLayoutGroup OrderList;
    public Image CurrentUnitView;
    #endregion

    /// <summary>
    /// 当前玩家角色
    /// </summary>
    Player _currentPlayer;

    IEnumerable<Vector2Int> _movePoint;
  
    List<Image> _orderListView = new();

    bool _enable = false;

    protected override void OnInit()
    {
        base.OnInit();
        var sta = ServiceFactory.Instance.GetService<GameManager>().GetState() as BattleState;
        var mr = sta.MapRenderer;
        var ur = sta.UnitRenderer;

        //绑定单位选择详情功能
        UnitView currentSelect = null;
        System.Action refresh = () =>
        {
            UnitDataView.Refresh(currentSelect.ViewData);
        };
        foreach (var view in ur.UnitViews)
        {
            view.OnClicked += () =>
            {
                if(currentSelect != null)
                {
                    currentSelect.ViewDataChanged -= refresh;
                }
                currentSelect = view;
                currentSelect.ViewDataChanged += refresh;

                UnitDataView.gameObject.SetActive(true);
                UnitDataView.RefreshWithoutAnim(view.ViewData);
            };
        }

        //绑定UI功能
        TogMove.onValueChanged.AddListener(v =>
        {
            if (v)
            {
                Hands.Disable();
                _movePoint = sta.CurrentUnit.GetMoveArea();
                mr.RenderMoveTile(_movePoint);
                BtnFinish.interactable = false;
            }
            else
            {
                Hands.Enable();
                mr.RenderMoveTile();
                BtnFinish.interactable = true;
            }
        });
        SwanSongRoundNumber.text = sta.SwanSongRoundNumber.ToString();

        BtnDeck.onClick.AddListener(() =>
        {
            Deck.gameObject.SetActive(true);
            IEnumerable<Card> cards = new List<Card>();
            foreach (var list in GameManager.Instance.GetState<BattleState>().PlayerList
                .Select(p => p.Scheduler.Deck))
            {
                cards = cards.Concat(list);
            }
            Deck.ShowCardList(cards);
        });
        BtnDiscard.onClick.AddListener(() =>
        {
            Discard.gameObject.SetActive(true);
            IEnumerable<Card> cards = new List<Card>();
            foreach (var list in GameManager.Instance.GetState<BattleState>().PlayerList
                .Select(p => p.Scheduler.DiscardPile))
            {
                cards = cards.Concat(list);
            }
            Discard.ShowCardList(cards);
        });
        BtnDiscardReturn.onClick.AddListener(() =>
        {
            Discard.gameObject.SetActive(false);
        });
        BtnDeckReturn.onClick.AddListener(() =>
        {
            Deck.gameObject.SetActive(false);
        });

        //绑定对局事件
        sta.Successed += () =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .OpenPanel("SuccessPanel");
        };
        sta.Failed += () =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .OpenPanel("FailurePanel");
        };
    }

    private void Update()
    {
        var mouse = TileUtility.GetMouseInCell();
        var pos = mouse.ToVector2Int();
        if (_enable && Mouse.current.leftButton.wasReleasedThisFrame && TileUtility.TryGetTile(pos, out var tile))
        {
            if (TogMove.isOn && _movePoint.Contains(pos))
            {
                _currentPlayer.Move(pos);
                TogMove.isOn = false;
            }
        }
    }

    public Tween NextUnit(List<Unit> orderList)
    {
        var anim = ExtensionDotween.GetEmptyTween(0.01f);
        anim.OnStart(() =>
        {
            if (_currentPlayer != null)
            {
                EndControl(_currentPlayer);
            }
            _currentPlayer = orderList.First() as Player;

            CurrentUnitView.sprite = orderList[0].UnitData.Face;
            orderList.RemoveAt(0);
            if (orderList.Count < _orderListView.Count)
            {
                for (int i = _orderListView.Count - 1; i >= orderList.Count; --i)
                {
                    Destroy(_orderListView[i].transform.parent.gameObject);
                    _orderListView.RemoveAt(i);
                }
            }
            else if(orderList.Count > _orderListView.Count)
            {
                for (int i = _orderListView.Count; i < orderList.Count; ++i)
                {
                    var obj = new GameObject("order", typeof(RectTransform), typeof(Image));
                    var obj2 = new GameObject("order", typeof(RectTransform), typeof(Image));
                    obj.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 0);
                    var rect = obj2.GetComponent<RectTransform>();
                    obj2.transform.SetParent(obj.transform, false);
                    obj.transform.SetParent(OrderList.transform, false);
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.offsetMax = Vector2.zero;
                    rect.offsetMin = Vector2.zero;
                    var img = obj2.GetComponent<Image>();
                    img.preserveAspect = true;
                    _orderListView.Add(img);
                }
            }
            for(int i = 0; i < orderList.Count; ++i)
            {
                _orderListView[i].sprite = orderList[i].UnitData.Face;
            }

            if(_currentPlayer != null)
            {
                BeginControl(_currentPlayer);
            }
        });
        return anim;
    }

    public Tween NextRound(int round)
    {
        var anim = ExtensionDotween.GetEmptyTween(0.01f);
        anim.OnStart(() =>
        {
            RoundNumber.text = $"{round}Turn";
        });
        return anim;
    }

    public Tween AddCard(Card card, CardScheduler cardScheduler)
    {
        var anim = ExtensionDotween.GetEmptyTween(0.01f);
        anim.OnStart(() =>
        {
            Hands.AddCard(card, cardScheduler);
        });
        return anim;
    }

    public Tween RemoveCard(Card card)
    {
        var anim = ExtensionDotween.GetEmptyTween(0.01f);
        anim.OnStart(() =>
        {
            Hands.RemoveCard(card);
        });
        return anim;
    }


    /// <summary>
    /// 启用玩家控制
    /// </summary>
    public void BeginControl(Player player)
    {
        TogMove.interactable = true;
        TogMove.SetIsOnWithoutNotify(false);
        BtnFinish.interactable = true;
        BtnFinish.onClick.AddListener(player.EndDecide);
        Enable();
    }

    /// <summary>
    /// 结束玩家控制
    /// </summary>
    public void EndControl(Player player)
    {
        TogMove.interactable = false;
        TogMove.SetIsOnWithoutNotify(false);
        BtnFinish.interactable = false;
        BtnFinish.onClick.RemoveListener(player.EndDecide);
        Disable();
    }

    public void Disable()
    {
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        Hands.Disable();
        _enable = false;
    }

    public void Enable()
    {
        CanvasGroup.interactable=true;
        CanvasGroup.blocksRaycasts=true;
        Hands.Enable();
        _enable = true;
    }
}
