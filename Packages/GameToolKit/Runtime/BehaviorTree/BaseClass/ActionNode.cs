using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.None)]
    [NodeCategory("Action")]
    [NodeColor("#0984e3")]
    public abstract class ActionNode : Leaf
    {
        
    }
}
