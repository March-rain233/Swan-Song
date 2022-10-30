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
    public HandsView Hands;
    public UnitDataView UnitDataView;

    public TextMeshProUGUI AP;
    public Button BtnFinish;
    public Toggle TogMove;

    public TextMeshProUGUI RoundNumber;
    public TextMeshProUGUI SwanSongRoundNumber;

    public CanvasGroup SuccessPanel;
    public CanvasGroup FailPanel;

    IEnumerable<Vector2Int> _movePoint;

    public HorizontalLayoutGroup OrderList;
    public Image CurrentUnitView;
    List<Image> _orderListView = new();

    protected override void OnInit()
    {
        base.OnInit();
        var sta = ServiceFactory.Instance.GetService<GameManager>().GetState() as BattleState;
        var mr = GameManager.Instance.GetState<BattleState>().MapRenderer;
        var players = sta.PlayerList;
        foreach (var player in players)
        {
            System.Action updataAP = () =>
            {
                AP.text = $"{player.UnitData.ActionPoint}/{player.UnitData.ActionPointMax}";
            };
            player.Scheduler.HandsAdded += (card)=>Hands.AddCard(card, player.Scheduler);
            player.Scheduler.HandsRemoved += Hands.RemoveCard;
            player.TurnBeginning += () =>
            {
                updataAP();
                Hands.UnblockCards();
                TogMove.interactable = true;
                TogMove.SetIsOnWithoutNotify(false);
                BtnFinish.interactable = true;
                BtnFinish.onClick.AddListener(player.EndDecide);
                player.UnitData.DataChanged += updataAP;
            };
            player.TurnEnding += () =>
            {
                Hands.BlockCards();
                TogMove.interactable = false;
                TogMove.SetIsOnWithoutNotify(false);
                BtnFinish.interactable = false;
                BtnFinish.onClick.RemoveListener(player.EndDecide);
                player.UnitData.DataChanged -= updataAP;
            };
        }

        TogMove.onValueChanged.AddListener(v =>
        {
            if (v)
            {
                Hands.BlockCards();
                _movePoint = sta.CurrentUnit.GetMoveArea();
                mr.RenderMoveTile(_movePoint);
                BtnFinish.interactable = false;
            }
            else
            {
                Hands.UnblockCards();
                mr.RenderMoveTile();
                BtnFinish.interactable = true;
            }
        });
        SwanSongRoundNumber.text = sta.SwanSongRoundNumber.ToString();

        sta.Successed += () =>
        {
            SuccessPanel.gameObject.SetActive(true);
            SuccessPanel.alpha = 0;
            SuccessPanel.DOFade(1, 0.3f);
            //todo
        };
        sta.Failed += () =>
        {
            FailPanel.gameObject.SetActive(true);
            FailPanel.alpha = 0;
            FailPanel.DOFade(1, 0.3f);
            //todo
        };
    }

    private void Update()
    {
        var mouse = TileUtility.GetMouseInCell();
        var pos = mouse.ToVector2Int();
        if (Mouse.current.leftButton.wasReleasedThisFrame && TileUtility.TryGetTile(pos, out var tile))
        {
            if(tile.Units.Count > 0)
            {
                UnitDataView.gameObject.SetActive(true);
                UnitDataView.BindingUnit(tile.Units[0].UnitData);
            }
            else
            {
                UnitDataView.gameObject.SetActive(false);
                if (TogMove.isOn && _movePoint.Contains(pos))
                {
                    var sta = ServiceFactory.Instance.GetService<GameManager>().GetState() as BattleState;
                    sta.CurrentUnit.Move(pos);
                    TogMove.isOn = false;
                }
            }
        }
    }

    public Tween NextUnit(List<Unit> orderList)
    {
        float temp = 0;
        var anim = DOTween.To(() => temp, (i) => temp = i, 1, 0.1f);
        anim.OnKill(() =>
        {
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
        });
        return anim;
    }

    public Tween NextRound(int round)
    {
        float temp = 0;
        var anim = DOTween.To(() => temp, (i) => temp = i, 1, 0.1f);
        anim.OnKill(() =>
        {
            RoundNumber.text = $"{round}Turn";
        });
        return anim;
    }

    public void Disable()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void Enable()
    {
        _canvasGroup.interactable=true;
        _canvasGroup.blocksRaycasts=true;
    }
}
