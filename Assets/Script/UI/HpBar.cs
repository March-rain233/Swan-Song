using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class HpBar : MonoBehaviour
{
    public RectMask2D HpMask;
    public RectMask2D HpBufferMask;
    public TextMeshProUGUI HpText;
    public Image HpView;

    public int MaxHp
    {
        get => _maxHp;
        set
        {
            SetMaxHp(value);
        }
    }
    int _maxHp;
    public int Hp
    {
        get => _hp;
        set
        {
            SetHp(value);
        }
    }
    int _hp;

    public Gradient HpColor;

    Tween _anim;

    public void SetMaxHp(int maxHp)
    {
        _maxHp = maxHp;
        UpdataBlood();
    }

    public void SetHp(int hp)
    {
        _hp = hp;
        UpdataBlood();
    }

    /// <summary>
    /// 设置血条百分比
    /// </summary>
    /// <param name="value"></param>
    public void UpdataBlood()
    {
        float percent = _hp / (float)_maxHp;
        float totalWidth = GetComponent<RectTransform>().rect.width;
        float padding = totalWidth * (1 - percent);
        var vec = new Vector4(0, 0, padding, 0);
        HpMask.padding = vec;
        _anim?.Kill();
        _anim = DOTween.To(() => HpBufferMask.padding,
            (v) => HpBufferMask.padding = v,
            vec,
            0.5f);
        HpText.text = $"{_hp}/{_maxHp}";
        HpView.color = HpColor.Evaluate(percent);
    }

    public void InitHpBar(int hp, int maxHp)
    {
        _hp = hp;
        _maxHp = maxHp;
        UpdataBlood();
        _anim?.Complete();
    }
}
