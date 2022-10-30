using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    [NodeCategory("Condition")]
    public abstract class ConditionNode : Leaf
    {
        public bool Invert = false;

        protected sealed override NodeStatus OnUpdate()
        {
            return Invert ^ OnCheck() ? NodeStatus.Success : NodeStatus.Failure;
        }
        protected abstract bool OnCheck();
    }
}
