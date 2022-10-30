using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit.Behavior.Tree {
    /// <summary>
    /// 行为节点基类
    /// </summary>
    public abstract class ProcessNode : Node
    {
        /// <summary>
        /// 节点状态
        /// </summary>
        [System.Serializable]
        public enum NodeStatus
        {
            /// <summary>
            /// 节点尚未运行过
            /// </summary>
            None,
            /// <summary>
            /// 节点运行成功
            /// </summary>
            Success,
            /// <summary>
            /// 节点运行失败
            /// </summary>
            Failure,
            /// <summary>
            /// 节点正在运行
            /// </summary>
            Running,
            /// <summary>
            /// 节点运行被打断
            /// </summary>
            Aborting,
        }

        /// <summary>
        /// 该节点运行是否还未结束
        /// </summary>
        public bool IsWorking => Status == NodeStatus.Running || Status == NodeStatus.Aborting;

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="node"></param>
        public abstract void AddChild(ProcessNode node);

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="node"></param>
        public abstract void RemoveChild(ProcessNode node);

        /// <summary>
        /// 更改子节点次序
        /// </summary>
        /// <param name="func"></param>
        public abstract void OrderChildren(Func<ProcessNode, ProcessNode, bool> func);

        /// <summary>
        /// 获取后继的行为节点
        /// </summary>
        /// <returns></returns>
        public abstract ProcessNode[] GetChildren();

        /// <summary>
        /// 当前状态
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
        /// 外界调用更新
        /// </summary>
        /// <returns>该节点更新后的状态</returns>
        public NodeStatus Tick()
        {
            InitInputData();
            //当节点并未处在运行状态时，进行进入状态处理
            if (Status != NodeStatus.Running)
            {
                //当节点处于被打断状态时恢复运行
                if (Status == NodeStatus.Aborting) OnResume();
                //否则节点处于已结算状态，重新进入运行状态
                else OnEnter();
                Status = NodeStatus.Running;
            }
            Status = OnUpdate();
            //当节点运行结束后，进行退出状态处理
            if (Status != NodeStatus.Running) 
                OnExit();
            //更新后继节点的数据
            InitOutputData();
            LastDataUpdataTime = Time.time;
            return Status;
        }

        /// <summary>
        /// 当该节点进入运行时调用
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// 当该节点退出运行时调用
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// 当该节点被打断时调用
        /// </summary>
        protected virtual void OnAbort() { }

        /// <summary>
        /// 当该节点恢复运行时调用
        /// </summary>
        protected virtual void OnResume() { }

        /// <summary>
        /// 打断当前节点的运行状态
        /// </summary>
        public void Abort()
        {
            //当节点未在运行时被打断输出错误信息
            if (Status != NodeStatus.Running)
            {
                Debug.Log($"{BehaviorTree}尝试打断{this}节点，但节点处于{Status}状态");
                return;
            }
            OnAbort();
            Status = NodeStatus.Aborting;
        }

        /// <summary>
        /// 当节点处于运行状态时调用
        /// </summary>
        /// <returns>本次运行的结果</returns>
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
