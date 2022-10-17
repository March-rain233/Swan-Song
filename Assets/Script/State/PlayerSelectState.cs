using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;

/// <summary>
/// ½ÇÉ«Ñ¡Ôñ×´Ì¬
/// </summary>
public class PlayerSelectState : GameState
{
    protected internal override void OnEnter()
    {
        ServiceFactory.Instance.GetService<PanelManager>().OpenPanel("PlayerSelectPanel");
    }

    protected internal override void OnExit()
    {
        ServiceFactory.Instance.GetService<PanelManager>().ClosePanel("PlayerSelectPanel");
    }

    protected internal override void OnUpdata()
    {
        
    }
}
