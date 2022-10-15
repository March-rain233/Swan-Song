using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 弧形布局元素接口
/// </summary>
public interface IArcLayoutElement
{
    /// <summary>
    /// 元素占据的角度
    /// </summary>
    public float Angle { get; set; }

    /// <summary>
    /// 元素最适的占据角度
    /// </summary>
    public float PreferAngle { get;}

    /// <summary>
    /// 元素最小的占据角度
    /// </summary>
    public float MinnumAngle { get;}

    /// <summary>
    /// 元素的灵活占据角度
    /// </summary>
    public float FlexibleAngle { get;}

    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(Vector2 position);

    /// <summary>
    /// 设置旋转角
    /// </summary>
    /// <param name="eulerAngle"></param>
    public void SetRotation(float eulerAngle);
}
