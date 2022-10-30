using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit.EventProcessor
{
    [NodeCategory("NULL")]
    public abstract class EventSenderNode : Node
    {
        /// <summary>
        /// 是否触发
        /// </summary>
        [Port("Invoke", PortDirection.Input)]
        [ReadOnly]
        public bool IsInvoke;
        public abstract System.Type EventType { get; }
    }
    /// <summary>
    /// 事件发送者
    /// </summary>
    public class EventSenderNode<TEventType> : EventSenderNode where TEventType : EventBase
    {
        [Port("Event", PortDirection.Input)]
        public TEventType EventArg;
        public override System.Type EventType => typeof(TEventType);
        protected override void OnValueUpdate()
        {
            if (IsInvoke)
            {
                Processor.SendEvent(EventArg);
            }
        }
        protected override void SetValue(string name, object value)
        {
            if(name == "IsInvoke")
            {
                IsInvoke = (bool)value;
            }
            else
            {
                var type = typeof(TEventType);
                type.GetField(name, System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance)
                    .SetValue(EventArg, value);
            }
        }
    }
}
