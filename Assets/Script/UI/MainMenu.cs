using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;
using TMPro;

public class MainMenu : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.HideOther;

    [SerializeField]
    Button _btnNewGame;
    [SerializeField]
    Button _btnQuit;
    [SerializeField]
    Button _btnContinue;

    public Button BtnSetting;

    public TextMeshProUGUI TxtVersion;

    protected override void OnInit()
    {
        base.OnInit();
        var state = ServiceFactory.Instance.GetService<GameManager>()
            .GetState() as MainMenuState;

        //°ó¶¨ÊÂ¼þ
        _btnNewGame.onClick.AddListener(()=>state.NewGame());
        _btnQuit.onClick.AddListener(()=>state.Quit());
        _btnContinue.onClick.AddListener(()=>state.Continue());
        BtnSetting.onClick.AddListener(() =>
        {
            ServiceFactory.Instance.GetService<PanelManager>().OpenPanel(nameof(SettingPanel));
        });
        _btnContinue.interactable = GameManager.Instance.HasSave;

        TxtVersion.text = $"Ver {Application.version}";
    }
}
