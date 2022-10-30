using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit.Behavior.Tree
{
    public class RepeaterNode : DecoratorNode
    {
        /// <summary>
        /// ���д���
        /// </summary>
        public int Times = 0;

        /// <summary>
        /// �ۼƴ���
        /// </summary>
        [ShowInInspector, ReadOnly]
        private int _add = 0;

        /// <summary>
        /// �Ƿ���������
        /// </summary>
        public bool IsForever = true;

        protected override void OnEnter()
        {
            base.OnEnter();
            _add = 0;
        }

        protected override NodeStatus OnUpdate()
        {
            Child.Tick();
            ++_add;
            if (IsForever || _add < Times)
            {
                return NodeStatus.Running;
            }
            return NodeStatus.Success;
        }
    }
}