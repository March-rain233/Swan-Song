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

    public bool IsChangingState { get; private set; } = false;

    public event Action<GameState> GameStateChanged;
    public event Action<Artifact> ArtifactAdded;

    /// <summary>
    /// 当前运行的游戏数据
    /// </summary>
    public GameData GameData
    {
        get => _gameData;
        internal set
        {
            if(_gameData != null)
            {
                foreach(var art in _gameData.Artifacts)
                {
                    art.Disable();
                }
            }
            _gameData = value;
            if (_gameData != null)
            {
                foreach (var art in _gameData.Artifacts)
                {
                    art.Enable();
                }
            }
        }
    }
    GameData _gameData;

    void IService.Init() 
    {
        var cs = ServiceFactory.Instance.GetService<PanelManager>()
            .Root.GetComponent<UnityEngine.UI.CanvasScaler>();
        cs.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        var objs = Resources.LoadAll<Sprite>("");
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
        if (IsChangingState)
        {
            return null;
        }
        IsChangingState = true;
        _gameStatus?.OnExit();
        _gameStatus = new TGameStatus() { _gameManager = this };
        _gameStatus.OnEnter();
        IsChangingState = false;
        
        GameStateChanged?.Invoke(_gameStatus);
        return _gameStatus as TGameStatus;
    }

    public void AddArtifact(Artifact artifact)
    {
        GameData.Artifacts.Add(artifact);
        artifact.Enable();
        ArtifactAdded?.Invoke(artifact);
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