using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public static class ExtensionDotween
{
    public static Tween GetEmptyTween(float duration)
    {
        float temp = 0;
        var anim = DOTween.To(() => temp, (i) => temp = i, 1, duration);
        return anim;
    }
}
