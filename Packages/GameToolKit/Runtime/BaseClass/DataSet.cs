using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Cinemachine;
using Sirenix.Serialization;

namespace GameToolKit
{
    /// <summary>
    /// 数据集
    /// </summary>
    [CreateAssetMenu(fileName = "Data Set", menuName = "Data Set/Data Set")]
    public class DataSet : SerializedScriptableObject, IBlackboard
    {
        [NoSaveDuringPlay]
        [NonSerialized, OdinSerialize]
        private Dictionary<string, BlackboardVariable> _variables = new Dictionary<string, BlackboardVariable>();
        [NonSerialized, OdinSerialize]
        private IBlackboard.CallBackList _callBackList = new IBlackboard.CallBackList();

        #region 变量增删查改
        public T GetValue<T>(string name)
        {
            return (T)_variables[name].Value;
        }

        public bool HasValue(string name)
        {
            return _variables.ContainsKey(name);
        }

        public void RemoveValue(string name)
        {
            _variables[name].ValueChanged -= Variable_ValueChanged;
            _variables.Remove(name);
            var e = new IBlackboard.ValueRemoveEvent(this, name);
            _callBackList.RemoveItem(name);
        }

        public void SetValue(string name, object value)
        {
            _variables[name].Value = value;
        }

        public void RenameValue(string oldName, string newName)
        {
            var temp = _variables[oldName];
            _variables.Remove(oldName);
            _variables.Add(newName, temp);
            var e = new IBlackboard.NameChangedEvent(this, newName, oldName);
            _callBackList.RenameItem(oldName, newName);
            _callBackList.Invoke(newName, e);
        }

        /// <summary>
        /// 获取已登记的变量列表
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, BlackboardVariable> GetVariables()
        {
            return new Dictionary<string, BlackboardVariable>(_variables);
        }

        public BlackboardVariable GetVariable(string name)
        {
            return _variables[name];
        }

        public void AddVariable(string name, BlackboardVariable variable)
        {
            _variables.Add(name, variable);
            variable.ValueChanged += Variable_ValueChanged; ;
        }

        private void Variable_ValueChanged(BlackboardVariable sender, object newValue, object oldValue)
        {
            foreach(var variable in _variables)
            {
                if(variable.Value == sender)
                {
                    var e = new IBlackboard.ValueChangeEvent(this, variable.Key, newValue, oldValue);
                    _callBackList.Invoke(variable.Key, e);
                    break;
                }
            }
        }
        #endregion

        #region 事件注册
        public void RegisterCallback<T>(string name, Action<T> callback) where T : IBlackboard.BlackboardEventBase
        {
            _callBackList.RegisterCallback(name, callback);
        }

        public void UnregisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : IBlackboard.BlackboardEventBase
        {
            _callBackList.UnregisterCallback(name, callback);
        }

        #endregion
    }
}