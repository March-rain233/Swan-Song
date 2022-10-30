using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree {

    /// <summary>
    /// 逻辑节点
    /// </summary>
    /// <remarks>
    /// 负责数据运算
    /// </remarks>
    [Node(NodeAttribute.PortType.None, NodeAttribute.PortType.None)]
    [NodeCategory("Logic")]
    [NodeColor("#81ecec")]
    public abstract class LogicNode : Node
    {

    }
}
