using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 逻辑节点
    /// </summary>
    /// <remarks>
    /// 负责数据运算
    /// </remarks>
    [Node(NodeAttribute.PortType.None, NodeAttribute.PortType.None)]
    [NodeCategory("Logic")]
    [NodeColor("#81ecec")]
    public abstract class LogicNode : Node
    {

    }

    [NodeCategory("Logic/Input")]
    public abstract class SourceNode : LogicNode
    {
        protected override void OnInit()
        {
            base.OnInit();
            InitOutputData();
        }
    }

    /// <summary>
    /// 拆分节点
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class SplitNode<TValue> : SourceNode
    {
        [Port("Value", PortDirection.Output, true)]
        [Port("Value", PortDirection.Input)]
        public TValue Value;
        protected override void OnValueUpdate()
        {

        }
    }

    /// <summary>
    /// 合并节点
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class CombineNode<TValue> : SourceNode
    {
        [Port("Value", PortDirection.Output)]
        [Port("Value", PortDirection.Input, true)]
        public TValue Value;
        protected override void OnValueUpdate()
        {

        }
    }

    /// <summary>
    /// 局部变量节点
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class InputNode<TValue> : SourceNode
    {
        [Port("Output", PortDirection.Output)]
        [SerializeField]
        [HideInInspector]
        protected TValue _value = default;
        [OdinSerialize]
        public TValue Value
        {
            get { return _value; }
            set
            {
                _value = value;
                InitOutputData();
            }
        }
        protected override void OnValueUpdate()
        {

        }

        protected override object GetValue(string name)
        {
            return _value;
        }
    }

    public class ToStringNode : LogicNode
    {
        [Port("Source", PortDirection.Input)]
        public object Source;

        [Port("Result", PortDirection.Output)]
        public string Result;
        protected override void OnValueUpdate()
        {
            Result = Source.ToString();
        }
    }

    public class StringConcatNode : LogicNode
    {
        [Port("SourceA", PortDirection.Input)]
        public string A;

        [Port("SourceB", PortDirection.Input)]
        public string B;

        [Port("Result", PortDirection.Output)]
        public string Result;

        protected override void OnValueUpdate()
        {
            Result = A + B;
        }
    }

    public abstract class CompareNode<TValue> : LogicNode
        where TValue : IComparable<TValue>
    {
        [Flags]
        public enum CompareType
        {
            Middle = 1,
            Bigger = 1 << 1,
            Lower = 1 << 2,
        }
        public CompareType Type;
        [Port("SourceA", PortDirection.Input)]
        public TValue SourceA;
        [Port("SourceB", PortDirection.Input)]
        public TValue SourceB;
        [Port("Result", PortDirection.Output)]
        public bool Result;

        protected override void OnValueUpdate()
        {
            int res = SourceA.CompareTo(SourceB);
            Result = (Type.HasFlag(CompareType.Middle) && res == 0)||
                (Type.HasFlag(CompareType.Bigger) && res == -1) ||
                (Type.HasFlag(CompareType.Lower) && res == 1);
        }
    }

    public class CompareIntNode : CompareNode<int> { }
    public class CompareFloatNode : CompareNode<float> { }
}
