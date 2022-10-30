using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit
{
    public struct SourceInfo
    {
        [SerializeField]
        BaseNode _sourceNode;
        [SerializeField]
        BaseNode _targetNode;
        [SerializeField]
        string _sourceField;
        [SerializeField]
        string _targetField;

        /// <summary>
        /// 源节点
        /// </summary>
        public BaseNode SourceNode { get { return _sourceNode; } }
        public BaseNode TargetNode { get { return _targetNode; } }
        /// <summary>
        /// 源数据段名称
        /// </summary>
        public string SourceField { get { return _sourceField; } }
        /// <summary>
        /// 目标数据段名称
        /// </summary>
        public string TargetField { get { return _targetField; } }

        public SourceInfo(BaseNode sourceNode, BaseNode targetNode, string sourceField, string targetField)
        {
            _sourceNode = sourceNode;
            _targetNode = targetNode;
            _sourceField = sourceField;
            _targetField = targetField;
        }

        public static bool operator ==(SourceInfo e1, SourceInfo e2)
        {
            return e1.SourceNode == e2.SourceNode && e1.TargetNode == e2.TargetNode && e1.SourceField == e2.SourceField && e1.TargetField == e2.TargetField;
        }

        public static bool operator !=(SourceInfo e1, SourceInfo e2)
        {
            return !(e1 == e2);
        }

        public override bool Equals(object obj)
        {
            return obj is SourceInfo info &&
                   EqualityComparer<BaseNode>.Default.Equals(_sourceNode, info._sourceNode) &&
                   EqualityComparer<BaseNode>.Default.Equals(_targetNode, info._targetNode) &&
                   _sourceField == info._sourceField &&
                   _targetField == info._targetField &&
                   EqualityComparer<BaseNode>.Default.Equals(SourceNode, info.SourceNode) &&
                   EqualityComparer<BaseNode>.Default.Equals(TargetNode, info.TargetNode) &&
                   SourceField == info.SourceField &&
                   TargetField == info.TargetField;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_sourceNode, _targetNode, _sourceField, _targetField, SourceNode, TargetNode, SourceField, TargetField);
        }
    }
}
