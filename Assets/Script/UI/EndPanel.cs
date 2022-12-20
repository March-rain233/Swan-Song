using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

internal class EndPanel : PanelBase
{
    public RectTransform Content;
    public TextMeshProUGUI TxtTip;
    public TextMeshProUGUI TxtThanks;
    public bool IsShowEnd = false;
    public float Speed = 50f;

    [Button]
    public void Begin()
    {
        IsShowEnd = false;

        TxtTip.gameObject.SetActive(false);
        TxtThanks.gameObject.SetActive(false);
        Content.gameObject.SetActive(true);

        Content.pivot = new Vector2(0.5f, 0f);
        Content.anchorMin = new Vector2(0, 1);
        Content.anchorMax = new Vector2(1, 1);
        Content.anchoredPosition = new Vector2(0, -Content.rect.height - (Content.parent as RectTransform).rect.height);
        Content.ForceUpdateRectTransforms();

        var anim = Content.DOAnchorPos3DY(0, Speed)
            .SetSpeedBased()
            .SetEase(Ease.Linear)
            .OnComplete(() =>
        {
            Content.gameObject.SetActive(false);
            TxtTip.gameObject.SetActive(true);
            TxtThanks.gameObject.SetActive(true);

            TxtThanks.DOFade(0, 0)
                .Complete();
            TxtTip.DOFade(0, 0)
                .Complete();

            TxtThanks.DOFade(1, BattleAnimator.MiddleAnimationDuration);
            TxtTip.DOFade(1, BattleAnimator.LongAnimationDuration)
                .SetLoops(-1, LoopType.Yoyo);

            IsShowEnd = true;
        });
    }

    private void Update()
    {
        if (IsShowEnd && Pointer.current.press.wasReleasedThisFrame)
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(this);
            GameManager.Instance.SetStatus<MainMenuState>();
        }
    }
}

