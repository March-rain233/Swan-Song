using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameToolKit.Editor
{
    /// <summary>
    /// ͼԪ�ؼ������ֶ�
    /// </summary>
    public abstract class GraphElementField : GraphElement
    {
        /// <summary>
        /// �鿴��Ԫ���Ƿ��봫����������
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool IsAssociatedWith(object obj);
    }
}
