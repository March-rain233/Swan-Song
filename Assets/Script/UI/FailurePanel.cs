using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;
using DG.Tweening;

public class FailurePanel : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.HideOther;

    public Button BtnReturnToTitle;

    protected override void OnInit()
    {
        base.OnInit();
        BtnReturnToTitle.onClick.AddListener(() =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(this);
            GameManager.Instance.SetStatus<MainMenuState>();
        });
    }
    protected override void OnOpen()
    {
        base.OnOpen();
        CanvasGroup.alpha = 0;
        CanvasGroup.DOFade(1, 0.3f);
    }
}
