﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolKit;

/// <summary>
/// 主界面状态
/// </summary>
public class MainMenuState : GameState
{
    protected internal override void OnEnter()
    {
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
        data.RandomState = UnityEngine.Random.state;
        data.TreeMap = TreeMapFactory.CreateTreeMap("");
        data.Members = new()
        {
            new UnitData(UnitSetting.Instance.UnitDic["Test"])
            {
                Name = "TestPlayer1",
                Deck = new() { new FireBall(), new FireBall(), new NormalAttack(), new NormalAttack() },
            },
            new UnitData(UnitSetting.Instance.UnitDic["Test"])
            {
                Name = "TestPlayer2",
                Deck = new() { new FireBall(), new FireBall(), new NormalAttack(), new NormalAttack() },
            }

        };
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
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void Quit()
    {
        UnityEngine.Application.Quit();
    }
}