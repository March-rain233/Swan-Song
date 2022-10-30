using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameToolKit
{
    /// <summary>
    /// �¼�����
    /// </summary>
    /// <remarks>
    /// ����ϵͳ�¼�������Ϸ��ʼ���������ε�<br></br>
    /// ������Ҫ����״̬���¼���ͨ�����ݿ������д���
    /// </remarks>
    public sealed class EventManager : IService
    {
        [ShowInInspector, ReadOnly]
        TypeTree _typeTree = new TypeTree(typeof(EventBase));

        [ShowInInspector, ReadOnly]
        Dictionary<Type, Delegate> _listeners = new Dictionary<Type, Delegate>();

        /// <summary>
        /// ע��ָ�����������¼��Ļص�
        /// </summary>
        /// <typeparam name="TEventType">�¼�����</typeparam>
        /// <param name="name">�¼�����</param>
        /// <param name="callback">�ص�����</param>
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
        /// ע��ָ�����������¼��Ļص�
        /// </summary>
        /// <typeparam name="TEventType">�¼�����</typeparam>
        /// <param name="name">�¼�����</param>
        /// <param name="callback">�ص�����</param>
        public void UnregisterCallback<TEventType>(Action<TEventType> callback) where TEventType : EventBase
        {
            var type = typeof(TEventType);
            if (_listeners.TryGetValue(type, out var action))
            {
                _listeners[type] = (Action<TEventType>)action - callback;
            }
        }

        /// <summary>
        /// �㲥�¼�
        /// </summary>
        /// <typeparam name="TEventType">�¼�����</typeparam>
        /// <param name="event">�¼�����</param>
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
        /// �õ����������Ľϸߣ���Ҫ�����ڱ༭���ڵ��ԣ���Ϸ��Ӧ����ʹ�÷��Ͱ汾
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
