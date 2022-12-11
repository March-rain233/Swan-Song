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
    DialogTree _dialog;
    protected internal override void OnEnter()
    {
        ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel(nameof(EventPanel));
    }

    protected internal override void OnExit()
    {
        ServiceFactory.Instance.GetService<DialogManager>()
            .CancelDialog(_dialog);
        ServiceFactory.Instance.GetService<PanelManager>()
            .ClosePanel(nameof(EventPanel));
    }

    protected internal override void OnUpdata()
    {

    }

    public void SetEvent(PlaceType placeType)
    {
        _dialog = EventSetting.Instance.EventDialogDic[placeType];
        ServiceFactory.Instance.GetService<DialogManager>()
            .PlayDialog(_dialog);
    }
}