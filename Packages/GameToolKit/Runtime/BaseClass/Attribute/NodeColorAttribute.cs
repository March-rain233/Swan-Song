using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit
{
    /// <summary>
    /// ������ͼ�нڵ㼰�ӽڵ����ɫ
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeColorAttribute : Attribute
    {
        Color _color;
        /// <summary>
        /// �ڵ���ɫ
        /// </summary>
        public Color Color => _color;
        public NodeColorAttribute(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out _color);
        }
    }
}
