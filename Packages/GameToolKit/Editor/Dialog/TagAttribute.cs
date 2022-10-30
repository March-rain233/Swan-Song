using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit.Dialog.Editor
{
    /// <summary>
    /// 标签视图声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TagAttribute : Attribute
    {
        public string MenuPath { get; private set; }

        public bool IsEmptyTag { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuPath">菜单路径</param>
        /// <param name="isEmptyTag">是否是空标签</param>
        public TagAttribute(string menuPath, bool isEmptyTag)
        {
            MenuPath = menuPath;
            IsEmptyTag = isEmptyTag;
        }
    }
}
