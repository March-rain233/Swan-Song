using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using DG.Tweening;
using UnityEngine.UI;

public class SuccessPanel : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.HideOther;

    public Button BtnNext;

    protected override void OnInit()
    {
        base.OnInit();
        BtnNext.onClick.AddListener(() =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(this);
            GameManager.Instance.SetStatus<SelectLevelState>();
        });
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        CanvasGroup.alpha = 0;
        CanvasGroup.DOFade(1, 0.3f);
    }
}
