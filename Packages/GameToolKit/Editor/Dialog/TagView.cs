using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameToolKit.Dialog.Editor
{
    /// <summary>
    /// ��ǩ��ͼ
    /// </summary>
    public abstract class TagView : VisualElement
    {
        /// <summary>
        /// ��Ӧ�ı�ǩ
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
        /// ͨ�������ı�����ֵ
        /// </summary>
        /// <param name="attr">�����ı�</param>
        /// <returns>�Ƿ�ת���ɹ�</returns>
        public abstract bool SetValueFromText(string attr);

        /// <summary>
        /// ת���������ı�
        /// </summary>
        /// <returns></returns>
        public abstract string ConvertToText();
    }
}
