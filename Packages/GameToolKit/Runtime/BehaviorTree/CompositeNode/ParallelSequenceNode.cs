using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 并行顺序节点
    /// </summary>
    public class ParallelSequenceNode : CompositeNode
    {
        protected override NodeStatus OnUpdate()
        {
            bool hasRunning = false;
            bool hasFailure = false;
            foreach (var child in Childrens)
            {
                switch (child.Tick())
                {
                    case NodeStatus.Failure:
                        hasFailure = true;
                        break;
                    case NodeStatus.Running:
                        hasRunning = true;
                        break;
                }
            }
            if (hasFailure) return NodeStatus.Failure;
            if (hasRunning) return NodeStatus.Running;
            return NodeStatus.Success;
        }
    }
}