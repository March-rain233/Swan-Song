using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using GameToolKit;
public class BattlePanel : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.Normal;
    public HandsView Hands;
    public UnitDataView UnitDataView;

    public TextMeshProUGUI AP;
    public Button BtnFinish;
    public Toggle TogMove;

    IEnumerable<Vector2Int> _movePoint;

    protected override void OnInit()
    {
        base.OnInit();
        var sta = ServiceFactory.Instance.GetService<GameManager>().GetState() as BattleState;
        var mr = GameManager.Instance.GetState<BattleState>().MapRenderer;
        var players = sta.PlayerList;
        foreach (var player in players)
        {
            player.Scheduler.HandsAdded += (card)=>Hands.AddCard(card, player.Scheduler);
            player.Scheduler.HandsRemoved += Hands.RemoveCard;
            player.TurnBeginning += () =>
            {
                AP.text = $"{player.UnitData.ActionPoint}/{player.UnitData.ActionPointMax}";
                Hands.UnblockCards();
                TogMove.interactable = true;
                TogMove.SetIsOnWithoutNotify(false);
                BtnFinish.interactable = true;
                BtnFinish.onClick.AddListener(player.EndDecide);
            };
            player.TurnEnding += () =>
            {
                Hands.BlockCards();
                TogMove.interactable = false;
                TogMove.SetIsOnWithoutNotify(false);
                BtnFinish.interactable = false;
                BtnFinish.onClick.RemoveListener(player.EndDecide);
            };
        }

        TogMove.onValueChanged.AddListener(v =>
        {
            if (v)
            {
                Hands.BlockCards();
                _movePoint = sta.CurrentUnit.GetMoveArea();
                mr.RenderMoveRange(_movePoint);
            }
            else
            {
                Hands.UnblockCards();
                mr.RenderMoveRange();
            }
        });
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
