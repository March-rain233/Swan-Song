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

    public HorizontalLayoutGroup OrderList;
    public Image CurrentUnitView;

    public LayoutGroup PlayerListView;
    public ToggleGroup PlayerGroup;
    #region 控件模板
    public GameObject UnitProfileModel;
    #endregion
    #endregion

    Dictionary<Player, Toggle> ToggleDic = new Dictionary<Player, Toggle>();
    public Color SelectColor;
    public Color UnselectColor;

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
        System.Action<UnitData> refreshData = (_) =>
        {
            UnitDataView.RefreshData(currentSelect.ViewData);
        };
        System.Action refreshBuff = () =>
        {
            UnitDataView.RefreshBuffList(currentSelect.BuffDatas);
        };
        foreach (var view in ur.UnitViews)
        {
            view.OnClicked += () =>
            {
                if(currentSelect != null)
                {
                    currentSelect.ViewDataChanged -= refreshData;
                    currentSelect.BuffChanged -= refreshBuff;
                }
                currentSelect = view;
                currentSelect.ViewDataChanged += refreshData;
                currentSelect.BuffChanged += refreshBuff;

                UnitDataView.gameObject.SetActive(true);
                UnitDataView.RefreshDataWithoutAnim(view.ViewData);
                refreshBuff();
            };
        }
        UnitDataView.transform.GetComponentInChildren<Button>()
            .onClick.AddListener(() =>
            {
                UnitDataView.gameObject.SetActive(false);
            });
        UnitDataView.gameObject.SetActive(false);

        //绑定玩家角色列表
        foreach(var player in sta.PlayerList)
        {
            var obj = Instantiate(UnitProfileModel, PlayerListView.transform);
            var face = obj.transform.Find("Face").GetComponent<Image>();
            var name = obj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var hpBar = obj.transform.Find("HpBar").GetComponent<HpBar>();
            var toggle = obj.GetComponent<Toggle>();
            var img = obj.GetComponent<Image>();
            face.sprite = player.UnitData.Face;
            name.text = player.UnitData.Name;
            hpBar.InitHpBar(player.UnitData.Blood, player.UnitData.BloodMax);
            ur.UnitViews.First(v => v.Unit == player).ViewDataChanged += (_) =>
              {
                  hpBar.MaxHp = player.UnitData.BloodMax;
                  hpBar.Hp = player.UnitData.Blood;
              };
            toggle.group = PlayerGroup;
            toggle.onValueChanged.AddListener((v) =>
            {
                if (v)
                {
                    img.color = SelectColor;
                    if (_currentPlayer != player)
                    {
                        SwitchPlayer(player);
                    }
                }
                else
                {
                    img.color = UnselectColor;
                }
            });
            ToggleDic[player] = toggle;
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
            var res = from player in GameManager.Instance.GetState<BattleState>().PlayerList
                      let list = from card in player.Scheduler.Deck
                                select (card, player.UnitData)
                      select (player.UnitData.Name, list);
            var panel = ServiceFactory.Instance.GetService<PanelManager>()
                .OpenPanel(nameof(DeckPanel)) as DeckPanel;
            panel.ShowCardList(res);
        });
        BtnDiscard.onClick.AddListener(() =>
        {
            var res = from player in GameManager.Instance.GetState<BattleState>().PlayerList
                      let list = from card in player.Scheduler.DiscardPile
                                 select (card, player.UnitData)
                      select (player.UnitData.Name, list);
            var panel = ServiceFactory.Instance.GetService<PanelManager>()
                .OpenPanel(nameof(DeckPanel)) as DeckPanel;
            panel.ShowCardList(res);
        });
    }

    private void Update()
    {
        var mouse = TileUtility.GetMouseInCell();
        var pos = mouse.ToVector2Int();
        if (_enable && Pointer.current.press.wasReleasedThisFrame && TileUtility.TryGetTile(pos, out var tile))
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
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            //刷新行动序列UI
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
            else if (orderList.Count > _orderListView.Count)
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
            for (int i = 0; i < orderList.Count; ++i)
            {
                _orderListView[i].sprite = orderList[i].UnitData.Face;
            }
        });
        return seq;
    }

    public Tween NextRound(int round)
    {
        var anim = ExtensionDotween.GetEmptyTween(0.01f);
        anim.OnStart(() =>
        {
            RoundNumber.text = $"回合 {round}";
        });
        return anim;
    }

    public Tween AddCard(Card card, CardScheduler cardScheduler)
    {
        var anim = ExtensionDotween.GetEmptyTween(0.01f);
        anim.OnStart(() =>
        {
            Hands.AddCard(card, cardScheduler);
            Hands.Refresh();
        });
        return anim;
    }

    public Tween RemoveCard(Card card, CardScheduler cardScheduler)
    {
        var anim = ExtensionDotween.GetEmptyTween(0.01f);
        anim.OnStart(() =>
        {
            Hands.DiscardCard(card, cardScheduler);
            Hands.Refresh();
        });
        return anim;
    }

    public Tween SwitchPlayer(Player player)
    {
        var seq = DOTween.Sequence(this);
        seq.AppendCallback(() =>
        {
            var ur = GameManager.Instance.GetState<BattleState>().UnitRenderer;
            if (_currentPlayer != null)
            {
                ur.UnitViews.First(v => v.Unit == _currentPlayer).ViewDataChanged -= BattlePanel_ViewDataChanged;
            }
            _currentPlayer = player;
            if (_currentPlayer != null)
            {
                ur.UnitViews.First(v => v.Unit == _currentPlayer).ViewDataChanged += BattlePanel_ViewDataChanged;
                BattlePanel_ViewDataChanged(ur.UnitViews.First(v => v.Unit == _currentPlayer).ViewData);
            }
            BtnFinish.interactable = TogMove.interactable = _enable && _currentPlayer.ActionStatus == ActionStatus.Running;
            ToggleDic[player].isOn = true;
        });
        seq.Append(Hands.SwitchPlayer(player));
        return seq;
    }

    private void BattlePanel_ViewDataChanged(UnitData obj)
    {
        AP.text = $"{obj.ActionPoint}/{obj.ActionPointMax}";
        Hands.Refresh();
    }

    /// <summary>
    /// 启用玩家控制
    /// </summary>
    public Tween BeginControl(Player player)
    {
        var seq = DOTween.Sequence();
        seq.Append(SwitchPlayer(player));
        seq.AppendCallback(()=>
        {
            TogMove.SetIsOnWithoutNotify(false);
            BtnFinish.onClick.AddListener(player.EndDecide);
            Enable();
        });
        return seq;
    }

    /// <summary>
    /// 结束玩家控制
    /// </summary>
    public Tween EndControl(Player player)
    {
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            _currentPlayer = null;
            TogMove.SetIsOnWithoutNotify(false);
            BtnFinish.onClick.RemoveListener(player.EndDecide);
            Disable();
        });
        return seq;
    }

    public void Disable()
    {
        _enable = false;
        BtnFinish.interactable = TogMove.interactable = false;
        Hands.Disable();
    }

    public void Enable()
    {
        _enable = true;
        BtnFinish.interactable = TogMove.interactable = _currentPlayer.ActionStatus == ActionStatus.Running;
        Hands.Enable();
    }
}
