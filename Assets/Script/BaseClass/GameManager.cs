using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameToolKit;
using UnityEngine;
using Newtonsoft.Json;
public class GameManager : IService
{
    public const int MaxLevel = 5;
    public static GameManager Instance => ServiceFactory.Instance.GetService<GameManager>();
    private GameState _gameStatus;

    public string SavePath => Application.persistentDataPath + "/save.json";
    public bool HasSave => GameData != null;

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
        var cs = ServiceFactory.Instance.GetService<PanelManager>()
            .Root.GetComponent<UnityEngine.UI.CanvasScaler>();
        cs.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

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

    public void SaveGame()
    {
        GameData.RandomState = UnityEngine.Random.state;
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.TypeNameHandling = TypeNameHandling.All;
        var data = JsonConvert.SerializeObject(GameData, settings);
        File.WriteAllText(SavePath, data);
        Debug.Log("Save Success");
    }

    public void LoadGame()
    {
        try
        {
            var json = File.ReadAllText(SavePath);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
            GameData = JsonConvert.DeserializeObject<GameData>(json, settings);
            UnityEngine.Random.state = GameData.RandomState;
        }
        catch (Exception ex)
        {
            GameData = null;
        }
    }
}