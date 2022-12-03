using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolKit;
public class GameManager : IService
{
    public const int MaxLevel = 5;
    public static GameManager Instance => ServiceFactory.Instance.GetService<GameManager>();
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
    public TGameState GetState<TGameState>() where TGameState : GameState
    {
        return _gameStatus as TGameState;
    }
    public TGameStatus SetStatus<TGameStatus>()where TGameStatus : GameState, new()
    {
        _gameStatus?.OnExit();
        _gameStatus = new TGameStatus() { _gameManager = this };
        _gameStatus.OnEnter();
        return _gameStatus as TGameStatus;
    }
}