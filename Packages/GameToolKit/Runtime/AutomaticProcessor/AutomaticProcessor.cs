using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEditor;
using System.Text.RegularExpressions;

namespace GameToolKit.EventProcessor
{
    /// <summary>
    /// 自动处理器
    /// </summary>
    /// <remarks>
    /// 根据系统中的变量与事件自动执行逻辑
    /// </remarks>
    [CreateAssetMenu(fileName = "EventProcessor", menuName = "EventManager/EventProcessor")]
    public class AutomaticProcessor : CustomGraph<Node>
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName;
        /// <summary>
        /// 事件类型
        /// </summary>
        [ReadOnly, ShowInInspector]
        public Type EventType => SenderNode.EventType;
        /// <summary>
        /// 入口节点
        /// </summary>
        public EventSenderNode SenderNode;
        /// <summary>
        /// 是否只触发一次
        /// </summary>
        public bool IsTrigger = false;
        /// <summary>
        /// 初始化处理器
        /// </summary>
        internal void Attach()
        {
            foreach(var node in Nodes)
            {
                node.Attach();
            }
        }

        /// <summary>
        /// 剥离处理器
        /// </summary>
        internal void Detach()
        {
           foreach (var node in Nodes)
            {
                node.Detach();
            }
        }
        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="event">时间参数</param>
        internal void SendEvent<TEventType>(TEventType @event) where TEventType : EventBase
        {
            ServiceFactory.Instance.GetService<EventManager>()
                .Broadcast(@event);
            if (IsTrigger)
            {
                EventProcessorManager.Instance.DetachProcessor(this);
            }
        }


        public override Node CreateNode(Type type)
        {
            var node = base.CreateNode(type);
#if UNITY_EDITOR
            if (type.IsSubclassOf(typeof(EventSenderNode)))
            {
                node.Name = "Sender";
            }
#endif
            typeof(Node).GetProperty("Processor").SetValue(node, this);
            return node;
        }

        public override string ToString()
        {
            return $"{name}({EventType.Name}.{EventName})";
        }
    }
}
