using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
/// <summary>
/// 弧形布局器
/// </summary>
[Serializable]
public class ArcLayout
{
    /// <summary>
    /// 对齐方式
    /// </summary>
    public enum Align
    {
        /// <summary>
        /// 以坐标轴为对称轴排布
        /// </summary>
        Middle,
        /// <summary>
        /// 以坐标轴为起始轴向左排布
        /// </summary>
        Left,
        /// <summary>
        /// 以坐标轴为起始轴向右排布
        /// </summary>
        Right
    }

    /// <summary>
    /// 布局元素列表
    /// </summary>
    [OdinSerialize]
    public List<IArcLayoutElement> Children = new();

    /// <summary>
    /// 圆心
    /// </summary>
    public Vector2 Center;

    /// <summary>
    /// 坐标轴相对于极坐标轴的偏移角度（角度制）
    /// </summary>
    public float AxisOffset;

    /// <summary>
    /// 元素相对于坐标轴的对齐方式
    /// </summary>
    public Align AxisAlign;

    /// <summary>
    /// 半径
    /// </summary>
    public float Radius;

    /// <summary>
    /// 所有元素所能占据的最大角度（角度制）
    /// </summary>
    public float MaxAngle;

    [Button]
    public void Refresh()
    {
        //为所有元素分配为最小角度
        float total = 0;
        for(int i = 0; i < Children.Count; ++i)
        {
            total += Children[i].Angle = Children[i].MinnumAngle;
        }
        //如果占据空间存在剩余，则平分给具有最适角度的元素
        if(total < MaxAngle)
        {
            float remain = MaxAngle - total;
            var list = Children.Where(e=>e.PreferAngle > e.MinnumAngle);
            var sum = list.Sum(e => e.PreferAngle - e.MinnumAngle);
            if(remain <= sum)
            {
                float unit = remain / list.Count();
                foreach(var child in list)
                {
                    child.Angle += unit;
                }
            }
            else
            {
                foreach(var child in list)
                {
                    child.Angle = child.PreferAngle;
                }
                //仍有剩余，则分配给灵活角度
                remain -= sum;
                list = Children.Where(e => e.FlexibleAngle > 0);
                sum = list.Sum(e=>e.FlexibleAngle);
                foreach(var child in list)
                {
                    child.Angle += remain * (child.FlexibleAngle / sum);
                }
            }
        }

        //根据元素的角度设置布局
        total = Children.Sum(e=>e.Angle);
        float start = AxisOffset;
        float sign = 0;
        switch (AxisAlign)
        {
            case Align.Middle:
                start += total / 2;
                sign = -1;
                break;
            case Align.Left:
                sign = -1;
                break;
            case Align.Right:
                sign = 1;
                break;
        }
        foreach(var c in Children)
        {
            SetElement(c, start + c.Angle / 2 * sign);
            start += c.Angle * sign;
        }
    }

    /// <summary>
    /// 设置元素
    /// </summary>
    /// <param name="index">元素索引</param>
    /// <param name="angle">元素锚点与圆心连线与极坐标轴的夹角</param>
    void SetElement(IArcLayoutElement element, float angle)
    {
        Vector2 pos = Center;
        float radian = angle * Mathf.Deg2Rad;
        pos.x += Mathf.Cos(radian) * Radius;
        pos.y += Mathf.Sin(radian) * Radius;
        element.SetPosition(pos);
        element.SetRotation(angle);
    }
}
