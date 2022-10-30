using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի��ڵ����
    /// </summary>
    [NodeCategory("NULL")]
    [Node]
    public abstract class Node : BaseNode
    {
        /// <summary>
        /// �󶨵ĶԻ���
        /// </summary>
        [HideInGraphInspector]
        [ReadOnly]
        public DialogTree DialogTree;
        /// <summary>
        /// ��ʼ��
        /// </summary>
        public virtual void Init()
        {
            OnInit();
        }

        /// <summary>
        /// ����ʼ��
        /// </summary>
        protected virtual void OnInit()
        {

        }
    }
    /// <summary>
    /// ���̽ڵ����
    /// </summary>
    [NodeCategory("Process")]
    [NodeColor("#74b9ff")]
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.Multi)]
    public abstract class ProcessNode : Node
    {
        /// <summary>
        /// �ڵ�״̬
        /// </summary>
        public enum NodeStatus
        {
            /// <summary>
            /// Ԥ���׶�
            /// </summary>
            Preparation,
            /// <summary>
            /// ����״̬
            /// </summary>
            Working,
            /// <summary>
            /// ���״̬
            /// </summary>
            Completion
        }

        public NodeStatus Status { get; protected set; } = NodeStatus.Preparation;

        /// <summary>
        /// ��̽ڵ�
        /// </summary>
        [HideInGraphInspector]
        public List<ProcessNode> Children = new List<ProcessNode>();

        /// <summary>
        /// ǰ���ڵ�
        /// </summary>
        [HideInGraphInspector]
        public List<ProcessNode> Parents = new List<ProcessNode>();


        public sealed override void Init()
        {
            Status = NodeStatus.Preparation;
            base.Init();
        }

        /// <summary>
        /// ����
        /// </summary>
        public void Play()
        {
            Status = NodeStatus.Working;
            OnPlay();
        }

        /// <summary>
        /// ������ʱ
        /// </summary>
        protected abstract void OnPlay();

        /// <summary>
        /// ��������
        /// </summary>
        public void Finish()
        {
            Status = NodeStatus.Completion;
            OnFinish();
            RunSubsequentNode();
        }

        /// <summary>
        /// ����������
        /// </summary>
        protected virtual void OnFinish() { }

        /// <summary>
        /// ���к�̽ڵ�
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
    /// �������
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
    /// �����յ�
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