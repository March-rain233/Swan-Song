using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话节点基类
    /// </summary>
    [NodeCategory("NULL")]
    [Node]
    public abstract class Node : BaseNode
    {
        /// <summary>
        /// 绑定的对话树
        /// </summary>
        [HideInGraphInspector]
        [ReadOnly]
        public DialogTree DialogTree;
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            OnInit();
        }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected virtual void OnInit()
        {

        }
    }
    /// <summary>
    /// 流程节点基类
    /// </summary>
    [NodeCategory("Process")]
    [NodeColor("#74b9ff")]
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.Multi)]
    public abstract class ProcessNode : Node
    {
        /// <summary>
        /// 节点状态
        /// </summary>
        public enum NodeStatus
        {
            /// <summary>
            /// 预备阶段
            /// </summary>
            Preparation,
            /// <summary>
            /// 工作状态
            /// </summary>
            Working,
            /// <summary>
            /// 完成状态
            /// </summary>
            Completion
        }

        public NodeStatus Status { get; protected set; } = NodeStatus.Preparation;

        /// <summary>
        /// 后继节点
        /// </summary>
        [HideInGraphInspector]
        public List<ProcessNode> Children = new List<ProcessNode>();

        /// <summary>
        /// 前驱节点
        /// </summary>
        [HideInGraphInspector]
        public List<ProcessNode> Parents = new List<ProcessNode>();


        public sealed override void Init()
        {
            Status = NodeStatus.Preparation;
            base.Init();
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Play()
        {
            Status = NodeStatus.Working;
            OnPlay();
        }

        /// <summary>
        /// 当运行时
        /// </summary>
        protected abstract void OnPlay();

        /// <summary>
        /// 结束运行
        /// </summary>
        public void Finish()
        {
            Status = NodeStatus.Completion;
            OnFinish();
            RunSubsequentNode();
        }

        /// <summary>
        /// 当结束运行
        /// </summary>
        protected virtual void OnFinish() { }

        /// <summary>
        /// 运行后继节点
        /// </summary>
        protected virtual void RunSubsequentNode()
        {
            foreach(var child in Children)
            {
                child?.Play();
            }
        }

        protected override sealed void OnValueUpdate()
        {
            Debug.LogError("It should not have happened, but it did.");
        }
        protected override sealed object PullValue(string fieldName)
        {
            if (LastDataUpdataTime != Time.time)
            {
                Debug.LogError("Invalid call order");
            }
            return GetValue(fieldName);
        }
        protected override sealed void PushValue(string fieldName, object value)
        {
            SetValue(fieldName, value);
        }
    }

    /// <summary>
    /// 进程起点
    /// </summary>
    [NodeCategory("NULL")]
    [NodeColor("#55efc4")]
    [Node(NodeAttribute.PortType.None, NodeAttribute.PortType.Multi)]
    public sealed class EntryNode : ProcessNode
    {
        protected override void OnPlay()
        {
            Finish();
        }
    }
    /// <summary>
    /// 进程终点
    /// </summary>
    [NodeCategory("NULL")]
    [NodeColor("#ff7675")]
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.None)]
    public sealed class ExitNode : ProcessNode
    {
        protected override void OnPlay()
        {
            Finish();
        }

        protected override void RunSubsequentNode()
        {
            DialogTree.Finish();
        }
    }
}