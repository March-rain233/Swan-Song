using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// È¡·´Æ÷
    /// </summary>
    public class InverterNode : DecoratorNode
    {
        protected override NodeStatus OnUpdate()
        {
            switch (Child.Tick())
            {
                case NodeStatus.Success:
                    return NodeStatus.Failure;
                case NodeStatus.Failure:
                    return NodeStatus.Success;
                case NodeStatus.Running:
                    break;
            }

            throw new ProcessException(this, "It should not have happened, but it did.");
        }
    }
}
