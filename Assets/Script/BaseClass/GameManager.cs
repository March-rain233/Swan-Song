using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolKit;
public class GameManager : IService
{
    private GameState _gameStatus;

    /// <summary>
    /// 当前运行的游戏数据
    /// </summary>
    public GameData GameData
    {
        get;
        internal set;
    }

    void IService.Init() 
    {
        SetStatus<MainMenuState>();
    }

    public GameState GetState()
    {
        return _gameStatus;
    }
    public TGameStatus SetStatus<TGameStatus>()where TGameStatus : GameState, new()
    {
        _gameStatus?.OnExit();
        _gameStatus = new TGameStatus();
        _gameStatus.OnEnter();
        return _gameStatus as TGameStatus;
    }
}