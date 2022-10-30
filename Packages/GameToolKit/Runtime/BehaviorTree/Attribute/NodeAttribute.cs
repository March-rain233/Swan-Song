using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// ������ͼ�нڵ�Ĺ�������
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeAttribute : Attribute
    {
        /// <summary>
        /// ���̶˿�����
        /// </summary>
        public enum PortType
        {
            None,
            Single,
            Multi,
        }

        PortType _inputPort;
        /// <summary>
        /// ����ڵ�����
        /// </summary>
        public PortType InputPort => _inputPort;

        PortType _outputPort;
        /// <summary>
        /// ����ڵ�����
        /// </summary>
        public PortType OutputPort => _outputPort;

        public NodeAttribute(PortType inputPort = PortType.None, PortType outputPort = PortType.None)
        {
            _inputPort = inputPort;
            _outputPort = outputPort;
        }
    }
}
