using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Serialization;
namespace GameToolKit
{
    /// <summary>
    /// �ڰ�ӿ�
    /// </summary>
    public interface IBlackboard
    {
        #region ���Ͷ���
        [System.Serializable]
        public abstract class BlackboardEventBase
        {
            public IBlackboard Target { get; private set; }
            public string Name { get; private set; }
            public BlackboardEventBase(IBlackboard target, string name)
            {
                Target = target;
                Name = name;
            }

        }
        [System.Serializable]
        public class NameChangedEvent:BlackboardEventBase
        {
            public string OldName { get; private set; }
            public NameChangedEvent(IBlackboard target, string name, string oldname):base(target, name)
            {
                OldName = oldname;
            }
        }
        [System.Serializable]
        public class ValueRemoveEvent : BlackboardEventBase
        {
            public ValueRemoveEvent(IBlackboard target, string name):base(target, name)
            {
            }
        }
        [System.Serializable]
        public class ValueChangeEvent : BlackboardEventBase
        {
            public object NewValue { get; private set; }
            public object OldValue { get; private set; }
            public ValueChangeEvent(IBlackboard target, string name, object newValue, object oldValue) :base(target, name)
            {
                NewValue = newValue;
                OldValue = oldValue;
            }
        }
        [System.Serializable]
        public class CallBackList
        {
            [OdinSerialize]
            Dictionary<string, Dictionary<Type, Delegate>> _callbacks = new Dictionary<string, Dictionary<Type, Delegate>>();

            #region �ص�������ɾ���
            /// <summary>
            /// ע��ص�
            /// </summary>
            /// <typeparam name="TEventType"></typeparam>
            /// <param name="name"></param>
            /// <param name="callback"></param>
            public void RegisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : BlackboardEventBase
            {
                var type = typeof(TEventType);
                if (_callbacks.TryGetValue(name, out var table))
                {
                    if (table.ContainsKey(type))
                    {
                        table[type] = (Action<TEventType>)table[type] + callback;
                    }
                    else
                    {
                        table[type] = callback;
                    }
                }
                else
                {
                    table = new Dictionary<Type, Delegate>();
                    table.Add(type, callback);
                    _callbacks.Add(name, table);
                }
            }

            /// <summary>
            /// ע���ص�
            /// </summary>
            /// <typeparam name="TEventType"></typeparam>
            /// <param name="name"></param>
            /// <param name="callback"></param>
            /// <exception cref="ArgumentNullException"></exception>
            public void UnregisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : BlackboardEventBase
            {
                if(_callbacks.TryGetValue(name, out var table))
                {
                    var type = typeof (TEventType);
                    if (table.ContainsKey(type))
                    {
                        table[type] = (Action<TEventType>)table[type] - callback;
                        if(table[type] == null)
                        {
                            table.Remove(type);
                        }
                        return;
                    }
                }
            }

            /// <summary>
            /// �Ƴ�ָ�������ȫ���ص�
            /// </summary>
            /// <param name="name"></param>
            public void RemoveItem(string name)
            {
                _callbacks.Remove(name);
            }

            /// <summary>
            /// ������ָ������
            /// </summary>
            /// <param name="oldName"></param>
            /// <param name="newName"></param>
            public void RenameItem(string oldName, string newName)
            {
                if(_callbacks.TryGetValue(oldName, out var table))
                {
                    _callbacks.Remove(oldName);
                    _callbacks[newName] = table;
                }
            }
            #endregion

            /// <summary>
            /// �����ص�
            /// </summary>
            /// <typeparam name="TEventType"></typeparam>
            /// <param name="name"></param>
            /// <param name="para"></param>
            /// <exception cref="ArgumentNullException"></exception>
            public void Invoke<TEventType>(string name, TEventType para)
            {
                if(_callbacks.TryGetValue(name, out var table))
                {
                    var type = typeof(TEventType);
                    if (table.ContainsKey(type))
                    {
                        ((Action<TEventType>)table[type])(para);
                        return;
                    }
                }
            }
        }
        #endregion

        #region �¼�ע��
        /// <summary>
        /// ע��ص�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : BlackboardEventBase;
        public void UnregisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : BlackboardEventBase;
        #endregion

        #region ��������
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="name"></param>
        public void RenameValue(string name, string newName);
        /// <summary>
        /// ��ȡָ������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BlackboardVariable GetVariable(string name);
        /// <summary>
        /// ��ӱ���
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variable"></param>
        public void AddVariable(string name, BlackboardVariable variable);
        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetValue<T>(string name);
        /// <summary>
        /// ���ñ���ֵ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, object value);
        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="name"></param>
        public void RemoveValue(string name);
        /// <summary>
        /// �ж��Ƿ���ڱ���
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasValue(string name);
        #endregion
    }
}
