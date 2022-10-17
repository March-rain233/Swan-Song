using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitView : MonoBehaviour
{
    public Animator Animator;
    public SpriteRenderer SpriteRenderer;

    public Transform AttackTargetFlag;
    public bool IsSelected { get; private set; } = false;

    private void Awake()
    {
        AttackTargetFlag.gameObject.SetActive(false);
        IsSelected = false;
        AttackTargetFlag.DOLocalMoveY(0.5f, 1f)
            .From(true)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void Select()
    {
        IsSelected = true;
        AttackTargetFlag.gameObject.SetActive(true);
    }

    public void Unselect()
    {
        IsSelected = false;
        AttackTargetFlag.gameObject.SetActive(false);
    }
}
