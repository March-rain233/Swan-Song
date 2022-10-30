using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 顺序节点
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        /// <summary>
        /// 当前运行节点
        /// </summary>
        [SerializeField]
        private int _current = 0;

        protected override void OnEnter()
        {
            _current = 0;
        }

        protected override NodeStatus OnUpdate()
        {
            //检查打断
            for (int i = 0; i < _current; ++i)
            {
                //如果该节点打断自身则检测高优先度条件节点，若存在返回成功则直接打断
                if ((AbortType & AbortType.Self) != 0)
                {
                    var condition = Childrens[i] as ConditionNode;
                    if (condition != null && condition.Tick() == NodeStatus.Failure)
                    {
                        Childrens[_current].Abort();
                        return NodeStatus.Aborting;
                    }
                    continue;
                }
                //如果子结合节点打断右方则，若存在返回成功则直接打断
                var composite = Childrens[i] as CompositeNode;
                if (composite != null && (composite.AbortType & AbortType.LowerPriority) != 0)
                {
                    var s = composite.Tick();
                    if (s == NodeStatus.Running || s == NodeStatus.Failure)
                    {
                        Childrens[_current].Abort();
                        _current = i;
                        return s;
                    }
                    continue;
                }
            }

            NodeStatus status;
            do
            {
                status = Childrens[_current].Tick();
            }
            while (status == NodeStatus.Success && ++_current < Childrens.Count);
            return status;
        }

        public override void OrderChildren(Func<ProcessNode, ProcessNode, bool> func)
        {
            if (IsWorking)
            {
                var cur = Childrens[_current];
                base.OrderChildren(func);
                _current = Childrens.IndexOf(cur);
            }
            else
            {
                base.OrderChildren(func);
            }
        }

        public override void RemoveChild(ProcessNode node)
        {
            if (IsWorking)
            {
                var cur = Childrens[_current];
                base.RemoveChild(node);
                if (cur == node)
                {
                    _current = 0;
                }
                else
                {
                    _current = Childrens.IndexOf(cur);
                }
            }
            else
            {
                base.RemoveChild(node);
            }
        }
    }
}
