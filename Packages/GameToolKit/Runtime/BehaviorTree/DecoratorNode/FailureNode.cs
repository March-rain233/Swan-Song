using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    public class FailureNode : DecoratorNode
    {
        /// <summary>
        /// ÓÀ¼ÙÆ÷
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        protected override NodeStatus OnUpdate()
        {
            Child.Tick();
            return NodeStatus.Failure;
        }
    }
}