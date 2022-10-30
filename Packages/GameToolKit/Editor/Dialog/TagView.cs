using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameToolKit.Dialog.Editor
{
    /// <summary>
    /// 标签视图
    /// </summary>
    public abstract class TagView : VisualElement
    {
        /// <summary>
        /// 对应的标签
        /// </summary>
        public abstract string Tag { get; } 

        public event System.Action ValueChanged;

        protected void SendChangedEvent()
        {
            ValueChanged?.Invoke();
        }

        public TagView()
        {
            style.marginBottom = style.marginRight = style.marginLeft = style.marginTop = 3;
        }

        /// <summary>
        /// 通过属性文本设置值
        /// </summary>
        /// <param name="attr">属性文本</param>
        /// <returns>是否转换成功</returns>
        public abstract bool SetValueFromText(string attr);

        /// <summary>
        /// 转化出属性文本
        /// </summary>
        /// <returns></returns>
        public abstract string ConvertToText();
    }
}
