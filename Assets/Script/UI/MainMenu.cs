using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;

public class MainMenu : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.HideOther;

    [SerializeField]
    Button _btnNewGame;
    [SerializeField]
    Button _btnQuit;
    [SerializeField]
    Button _btnContinue;

    protected override void OnInit()
    {
        base.OnInit();
        var state = ServiceFactory.Instance.GetService<GameManager>()
            .GetState() as MainMenuState;

        //°ó¶¨ÊÂ¼þ
        _btnNewGame.onClick.AddListener(()=>state.NewGame());
        _btnQuit.onClick.AddListener(()=>state.Quit());
        _btnContinue.onClick.AddListener(()=>state.Continue());
    }
}
