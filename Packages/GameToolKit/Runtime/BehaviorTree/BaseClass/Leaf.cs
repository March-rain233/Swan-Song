using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameToolKit.Behavior.Tree
{
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.None)]
    [NodeColor("#55efc4")]
    public abstract class Leaf : ProcessNode
    {
        public override void AddChild(ProcessNode node)
        {
            Debug.LogError("不允许向叶子节点添加子节点");
        }
        public override void RemoveChild(ProcessNode node)
        {
            Debug.LogError("不允许向叶子节点移除子节点");
        }

        public override void OrderChildren(Func<ProcessNode, ProcessNode, bool> func)
        {
            
        }

        public override ProcessNode[] GetChildren()
        {
            return new ProcessNode[] {};
        }
    }
}
