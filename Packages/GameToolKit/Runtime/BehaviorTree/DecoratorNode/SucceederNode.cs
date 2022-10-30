using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// �ɹ���
    /// </summary>
    public class SucceederNode : DecoratorNode
    {
        protected override NodeStatus OnUpdate()
        {
            Child.Tick();
            return NodeStatus.Success;
        }
    }
}