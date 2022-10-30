using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit.Dialog.Editor
{
    /// <summary>
    /// ��ǩ��ͼ����
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TagAttribute : Attribute
    {
        public string MenuPath { get; private set; }

        public bool IsEmptyTag { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuPath">�˵�·��</param>
        /// <param name="isEmptyTag">�Ƿ��ǿձ�ǩ</param>
        public TagAttribute(string menuPath, bool isEmptyTag)
        {
            MenuPath = menuPath;
            IsEmptyTag = isEmptyTag;
        }
    }
}
