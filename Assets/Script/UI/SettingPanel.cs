using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;

internal class SettingPanel : PanelBase
{
    public Button BtnClose;
    public Button BtnToTitle;
    public Button BtnResetGuide;

    protected override void OnInit()
    {
        base.OnInit();
        var pm = ServiceFactory.Instance.GetService<PanelManager>();
        BtnClose.onClick.AddListener(() =>
        {
            pm.ClosePanel(this);
        });
        BtnToTitle.onClick.AddListener(() =>
        {
            pm.ClosePanel(this);
            GameManager.Instance.SetStatus<MainMenuState>();
        });
        BtnResetGuide.onClick.AddListener(() =>
        {
            ServiceFactory.Instance.GetService<GuiderManager>()
                .Reset();
        });
    }
}

