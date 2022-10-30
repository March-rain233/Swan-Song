using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 定义视图中节点的固有属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeAttribute : Attribute
    {
        /// <summary>
        /// 流程端口类型
        /// </summary>
        public enum PortType
        {
            None,
            Single,
            Multi,
        }

        PortType _inputPort;
        /// <summary>
        /// 输入节点类型
        /// </summary>
        public PortType InputPort => _inputPort;

        PortType _outputPort;
        /// <summary>
        /// 输出节点类型
        /// </summary>
        public PortType OutputPort => _outputPort;

        public NodeAttribute(PortType inputPort = PortType.None, PortType outputPort = PortType.None)
        {
            _inputPort = inputPort;
            _outputPort = outputPort;
        }
    }
}
