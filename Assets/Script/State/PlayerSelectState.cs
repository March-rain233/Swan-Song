using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using System.Linq;

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

    public void SetTeam(List<UnitModel> team)
    {
        _gameManager.GameData.Members = team.Select(m=>new UnitData(m)).ToList();
        _gameManager.SetStatus<SelectLevelState>();
    }

    protected internal override void OnUpdata()
    {
        
    }
}
