using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolKit;
/// <summary>
/// 关卡选择状态
/// </summary>
public class SelectLevelState : GameState
{
    protected internal override void OnEnter()
    {
        var map = GameManager.Instance.GameData.TreeMap;
        if(map.CurrentNode.PlaceType == PlaceType.BossBattle)
        {
            GameManager.Instance.GameData.Chapter += 1;
            GameManager.Instance.GameData.TreeMap = TreeMapFactory.CreateTreeMap("");
        }
        ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel("TreeMapView");
    }

    protected internal override void OnExit()
    {
        ServiceFactory.Instance.GetService<PanelManager>()
            .ClosePanel("TreeMapView");
    }

    protected internal override void OnUpdata()
    {

    }

    /// <summary>
    /// 选择前往的节点
    /// </summary>
    /// <param name="id">节点id</param>
    public void SelectNode(int id)
    {
        var gm = ServiceFactory.Instance.GetService<GameManager>();
        var map = gm.GameData.TreeMap;
        var node = map.FindNode(id);
        map.CurrentId = id;
        switch (node.PlaceType)
        {
            case PlaceType.NormalBattle:
                gm.SetStatus<BattleState>()
                    .InitSystem(gm.GameData.Chapter, 1);
                break;
            case PlaceType.AdvancedBattle:
                gm.SetStatus<BattleState>()
                    .InitSystem(gm.GameData.Chapter, 2);
                break;
            case PlaceType.BossBattle:
                gm.SetStatus<BattleState>()
                    .InitSystem(gm.GameData.Chapter, 3);
                break;
            case PlaceType.Start:
                break;
            default:
                gm.SetStatus<EventState>()
                    .SetEvent(node.PlaceType);
                break;
        }
    }
}