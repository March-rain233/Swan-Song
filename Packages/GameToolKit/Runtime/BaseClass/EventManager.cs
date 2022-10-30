using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameToolKit
{
    /// <summary>
    /// 事件管线
    /// </summary>
    /// <remarks>
    /// 发送系统事件，如游戏开始、输入屏蔽等<br></br>
    /// 所有需要储存状态的事件请通过数据库来进行传递
    /// </remarks>
    public sealed class EventManager : IService
    {
        [ShowInInspector, ReadOnly]
        TypeTree _typeTree = new TypeTree(typeof(EventBase));

        [ShowInInspector, ReadOnly]
        Dictionary<Type, Delegate> _listeners = new Dictionary<Type, Delegate>();

        /// <summary>
        /// 注册指定类型名称事件的回调
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="name">事件名称</param>
        /// <param name="callback">回调函数</param>
        public void RegisterCallback<TEventType>(Action<TEventType> callback) where TEventType : EventBase
        {
            var type = typeof(TEventType);
            _typeTree.TryAddType(type);
            if(_listeners.TryGetValue(type, out var action))
            {
                _listeners[type] = (Action<TEventType>)action + callback;
            }
            else
            {
                _listeners.Add(type, callback);
            }
        }

        /// <summary>
        /// 注销指定类型名称事件的回调
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="name">事件名称</param>
        /// <param name="callback">回调函数</param>
        public void UnregisterCallback<TEventType>(Action<TEventType> callback) where TEventType : EventBase
        {
            var type = typeof(TEventType);
            if (_listeners.TryGetValue(type, out var action))
            {
                _listeners[type] = (Action<TEventType>)action - callback;
            }
        }

        /// <summary>
        /// 广播事件
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="event">事件参数</param>
        public void Broadcast<TEventType>(TEventType @event) where TEventType : EventBase
        {
            var list = _typeTree.GetAncestor(typeof(TEventType));
            foreach(var type in list)
            {
                if (_listeners.TryGetValue(type, out var action))
                {
                    ((Action<TEventType>)action)(@event);
                }
            }
        }

        /// <summary>
        /// <inheritdoc cref="Broadcast{TEventType}(TEventType)"/>
        /// </summary>
        /// <remarks>
        /// 该调用性能消耗较高，主要用于在编辑器内调试，游戏内应用请使用泛型版本
        /// </remarks>
        /// <param name="event"></param>
        [Button]
        public void Broadcast(EventBase @event)
        {
            var paraType = @event.GetType();
            var list = _typeTree.GetAncestor(paraType);
            var actionType = typeof(Action<>);
            actionType.MakeGenericType(paraType);
            foreach (var type in list)
            {
                if (_listeners.TryGetValue(type, out var action))
                {
                    action.DynamicInvoke(@event);
                }
            }
        }
    }
}
