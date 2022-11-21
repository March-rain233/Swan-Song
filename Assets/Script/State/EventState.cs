using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolKit;
using GameToolKit.Dialog;

/// <summary>
/// 地图事件状态
/// </summary>
public class EventState : GameState
{
    protected internal override void OnEnter()
    {
        ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel("EventPanel");
    }

    protected internal override void OnExit()
    {
        ServiceFactory.Instance.GetService<PanelManager>()
            .ClosePanel("EventPanel");
    }

    protected internal override void OnUpdata()
    {

    }

    public void SetEvent(PlaceType placeType)
    {
        var dialog = EventSetting.Instance.EventDialogDic[placeType];
        ServiceFactory.Instance.GetService<DialogManager>()
            .PlayDialog(dialog);
    }
}