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
        //�������˿�ѡ���ͼ�飬�Ҹ�ͼ�黹δ����λռ�ݣ��򽫵�ǰѡ�е�λ�����ڸ�ͼ��
        //�����ͼ��ռ�ݵĵ�λ���ǵ�ǰ��λ����ȡ����λ��ս״̬
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
