using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolKit;
using UnityEngine;

/// <summary>
/// 主界面状态
/// </summary>
public class MainMenuState : GameState
{
    protected internal override void OnEnter()
    {
        GameManager.Instance.LoadGame();
        ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel("MainMenu");
    }

    protected internal override void OnExit()
    {
        ServiceFactory.Instance.GetService<PanelManager>()
            .ClosePanel("MainMenu");
    }

    protected internal override void OnUpdata()
    {
    }

    /// <summary>
    /// 开始新的游戏
    /// </summary>
    public void NewGame()
    {
        GameData data = new GameData();
        Random.InitState(Mathf.RoundToInt(Time.unscaledTime));
        data.RandomState = Random.state;
        data.TreeMap = TreeMapFactory.CreateTreeMap("");
        data.Members = new();
        data.Chapter = 1;
        data.Gold = 30;
        ServiceFactory.Instance.GetService<GameManager>()
            .GameData = data;

        ServiceFactory.Instance.GetService<GameManager>()
            .SetStatus<PlayerSelectState>();
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void Continue()
    {
        ServiceFactory.Instance.GetService<GameManager>()
            .SetStatus<SelectLevelState>();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}