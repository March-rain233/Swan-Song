using System.Collections;
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

    protected override void OnInit()
    {
        base.OnInit();
        var sta = ServiceFactory.Instance.GetService<GameManager>().GetState() as BattleState;
        var players = sta.PlayerList;
        foreach (var player in players)
        {
            player.Scheduler.DrewCard += (card)=>Hands.AddCard(card, player.Scheduler);
        }
    }

    private void Update()
    {
        var mouse = TileUtility.GetMouseInCell();
        var pos = new Vector2Int(mouse.x, mouse.y);
        //如果点击了可选择的图块，且该图块还未被单位占据，则将当前选中单位放置于该图块
        //如果该图块占据的单位就是当前单位，则取消单位参战状态
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
            }
        }
    }
}
