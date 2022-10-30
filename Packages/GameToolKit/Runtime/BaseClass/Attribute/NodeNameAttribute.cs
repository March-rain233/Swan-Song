using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// ����ڵ���ʾ��Ĭ������
    /// </summary>
    /// <remarks>
    /// ������Ÿ�������Ĭ����ʾ����
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeNameAttribute : Attribute
    {
        string _name;
        public string Name => _name;
        public NodeNameAttribute(string name)
        {
            _name = name;
        }
    }
}
