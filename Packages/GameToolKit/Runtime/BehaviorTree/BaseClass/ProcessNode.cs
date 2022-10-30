using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit.Behavior.Tree {
    /// <summary>
    /// ��Ϊ�ڵ����
    /// </summary>
    public abstract class ProcessNode : Node
    {
        /// <summary>
        /// �ڵ�״̬
        /// </summary>
        [System.Serializable]
        public enum NodeStatus
        {
            /// <summary>
            /// �ڵ���δ���й�
            /// </summary>
            None,
            /// <summary>
            /// �ڵ����гɹ�
            /// </summary>
            Success,
            /// <summary>
            /// �ڵ�����ʧ��
            /// </summary>
            Failure,
            /// <summary>
            /// �ڵ���������
            /// </summary>
            Running,
            /// <summary>
            /// �ڵ����б����
            /// </summary>
            Aborting,
        }

        /// <summary>
        /// �ýڵ������Ƿ�δ����
        /// </summary>
        public bool IsWorking => Status == NodeStatus.Running || Status == NodeStatus.Aborting;

        /// <summary>
        /// ����ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public abstract void AddChild(ProcessNode node);

        /// <summary>
        /// �Ƴ��ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public abstract void RemoveChild(ProcessNode node);

        /// <summary>
        /// �����ӽڵ����
        /// </summary>
        /// <param name="func"></param>
        public abstract void OrderChildren(Func<ProcessNode, ProcessNode, bool> func);

        /// <summary>
        /// ��ȡ��̵���Ϊ�ڵ�
        /// </summary>
        /// <returns></returns>
        public abstract ProcessNode[] GetChildren();

        /// <summary>
        /// ��ǰ״̬
        /// </summary>
        public NodeStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
#if UNITY_EDITOR
                switch (_status)
                {
                    case NodeStatus.Success:
                        ChangeColor(Color.green);
                        break;
                    case NodeStatus.Failure:
                        ChangeColor(Color.red);
                        break;
                    case NodeStatus.Running:
                        ChangeColor(Color.blue);
                        break;
                    case NodeStatus.Aborting:
                        ChangeColor(Color.yellow);
                        break;
                    default:
                        ChangeColor(Color.gray);
                        break;
                }
#endif
            }
        }
        NodeStatus _status = NodeStatus.None;

        /// <summary>
        /// �����ø���
        /// </summary>
        /// <returns>�ýڵ���º��״̬</returns>
        public NodeStatus Tick()
        {
            InitInputData();
            //���ڵ㲢δ��������״̬ʱ�����н���״̬����
            if (Status != NodeStatus.Running)
            {
                //���ڵ㴦�ڱ����״̬ʱ�ָ�����
                if (Status == NodeStatus.Aborting) OnResume();
                //����ڵ㴦���ѽ���״̬�����½�������״̬
                else OnEnter();
                Status = NodeStatus.Running;
            }
            Status = OnUpdate();
            //���ڵ����н����󣬽����˳�״̬����
            if (Status != NodeStatus.Running) 
                OnExit();
            //���º�̽ڵ������
            InitOutputData();
            LastDataUpdataTime = Time.time;
            return Status;
        }

        /// <summary>
        /// ���ýڵ��������ʱ����
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// ���ýڵ��˳�����ʱ����
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// ���ýڵ㱻���ʱ����
        /// </summary>
        protected virtual void OnAbort() { }

        /// <summary>
        /// ���ýڵ�ָ�����ʱ����
        /// </summary>
        protected virtual void OnResume() { }

        /// <summary>
        /// ��ϵ�ǰ�ڵ������״̬
        /// </summary>
        public void Abort()
        {
            //���ڵ�δ������ʱ��������������Ϣ
            if (Status != NodeStatus.Running)
            {
                Debug.Log($"{BehaviorTree}���Դ��{this}�ڵ㣬���ڵ㴦��{Status}״̬");
                return;
            }
            OnAbort();
            Status = NodeStatus.Aborting;
        }

        /// <summary>
        /// ���ڵ㴦������״̬ʱ����
        /// </summary>
        /// <returns>�������еĽ��</returns>
        protected abstract NodeStatus OnUpdate();

        protected override sealed void OnValueUpdate()
        {
            throw new ProcessException(this, "It should not have happened, but it did.");
        }

        protected override sealed object PullValue(string fieldName)
        {
            if (LastDataUpdataTime != Time.time)
            {
                throw new ProcessException(this, "Invalid call order");
            }
            return GetValue(fieldName);
        }

        protected override sealed void PushValue(string fieldName, object value)
        {
            SetValue(fieldName, value);
        }
    }
}
