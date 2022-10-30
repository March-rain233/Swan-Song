using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameToolKit.Editor
{
    /// <summary>
    /// 图元素监视器字段
    /// </summary>
    public abstract class GraphElementField : GraphElement
    {
        /// <summary>
        /// 查看该元素是否与传入对象相关联
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool IsAssociatedWith(object obj);
    }
}
