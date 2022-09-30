using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameManager
{
    private GameStatus _gameStatus;

    /// <summary>
    /// 当前运行的游戏数据
    /// </summary>
    public GameData GameData
    {
        get => default;
        set
        {
        }
    }

    public GameStatus GetStatus()
    {
        throw new System.NotImplementedException();
    }
    public TGameStatus SetStatus<TGameStatus>()where TGameStatus : GameStatus
    {
        throw new System.NotImplementedException();
    }
}