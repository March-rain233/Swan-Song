using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit
{
    /// <summary>
    /// 定义视图中节点及子节点的颜色
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeColorAttribute : Attribute
    {
        Color _color;
        /// <summary>
        /// 节点颜色
        /// </summary>
        public Color Color => _color;
        public NodeColorAttribute(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out _color);
        }
    }
}
