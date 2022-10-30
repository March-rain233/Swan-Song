using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit
{
    /// <summary>
    /// 定义节点及子类的分类路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeCategoryAttribute : Attribute
    {
        string _category;
        public string Category => _category;
        public NodeCategoryAttribute(string category)
        {
            _category = category;
        }
    }
}
