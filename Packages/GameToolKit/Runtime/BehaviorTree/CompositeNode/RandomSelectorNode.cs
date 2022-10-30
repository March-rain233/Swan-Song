using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 随机选择节点
    /// </summary>
    public class RandomSelectorNode : CompositeNode
    {
        [System.Serializable]
        private class WeightInfo
        {
            /// <summary>
            /// 当前权重
            /// </summary>
            public int Weight;
            /// <summary>
            /// 基础权重
            /// </summary>
            public int BaseWeight;
            /// <summary>
            /// 权重每一次上升的对应值
            /// </summary>
            public int DeltaWeight;
            /// <summary>
            /// 权重是否会重置
            /// </summary>
            public bool IsReset;
        }

        private int _current;

        /// <summary>
        /// 节点随机到的权重
        /// </summary>
        /// <remarks>
        /// 权重越高抽到的概率越大
        /// </remarks>
        [SerializeField]
        private List<WeightInfo> _weights;

        private int _totalWight
        {
            get
            {
                int total = 0;
                _weights.ForEach(w => total += w.Weight);
                return total;
            }
        }
        public override void AddChild(ProcessNode node)
        {
            base.AddChild(node);
            _weights.Add(new WeightInfo());
        }
        public override void RemoveChild(ProcessNode node)
        {
            int i = Childrens.IndexOf(node);
            base.RemoveChild(node);
            _weights.RemoveAt(i);
        }
        protected override void OnEnter()
        {
            _current = -1;
            int total = _totalWight;
            int value = Random.Range(0, total + 1);
            for(int i = 0; i < _weights.Count; ++i)
            {
                if (_current < 0 && value <= _weights[i].Weight)
                {
                    _current = i;
                    if (!_weights[i].IsReset) { _weights[i].Weight = _weights[i].BaseWeight; }
                    continue;
                }
                value -= _weights[i].Weight;
                _weights[i].Weight += _weights[i].DeltaWeight;
            }
        }

        protected override NodeStatus OnUpdate()
        {
            return Childrens[_current].Tick();
        }
    }
}
