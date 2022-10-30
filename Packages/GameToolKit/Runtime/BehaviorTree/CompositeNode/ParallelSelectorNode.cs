using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 并行选择节点
    /// </summary>
    public class ParallelSelectorNode : CompositeNode
    {

        protected override NodeStatus OnUpdate()
        {
            bool hasRunning = false;
            bool hasSuccess = false;
            foreach(var child in Childrens)
            {
                switch (child.Tick())
                {
                    case NodeStatus.Success:
                        hasSuccess = true;
                        break;
                    case NodeStatus.Running:
                        hasRunning = true;
                        break;
                }
            }
            if (hasSuccess) return NodeStatus.Success;
            if (hasRunning) return NodeStatus.Running;
            return NodeStatus.Failure;
        }
    }
}