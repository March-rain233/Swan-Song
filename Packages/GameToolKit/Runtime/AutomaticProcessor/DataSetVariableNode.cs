using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

namespace GameToolKit.EventProcessor
{
    public class DataSetVariableNode<T> : InputNode<T>
    {
        [SerializeField, HideInInspector]
        private string _dataSetName = "";

        /// <summary>
        /// 数据集名称
        /// </summary>
        [OdinSerialize]
        [DelayedProperty]
        [ValueDropdown("GetValidDataSetIndex", AppendNextDrawer = true)]
        [InfoBox("The DataSet is not exist", InfoMessageType.Warning, "IsNotContainDataSet")]
        public string DataSetName
        {
            get { return _dataSetName; }
            set
            {
                var old = _dataSetName;
                if (old == value)
                {
                    return;
                }
                else
                {
                    if (old != null)
                    {
                        Index = "";
                    }
                    _dataSetName = value;
                    if (old != null)
                    {
                        DataSetManager.Instance.TryGetDataSet(_dataSetName, out _dataSet);
                    }
                }
            }
        }
        protected IBlackboard _blackboard => _dataSet;
        private DataSet _dataSet;
        [OdinSerialize]
        [HideInInspector]
        private string _index = "";

        [OdinSerialize]
        [ValueDropdown("GetValidIndex", AppendNextDrawer = true)]
        [InfoBox("The index is not contained in the dataset", InfoMessageType.Warning, "IsNotContainIndex")]
        [DelayedProperty]
        public string Index
        {
            get { return _index; }
            set
            {
                var old = _index;
                if (!string.IsNullOrEmpty(_index))
                {
                    _blackboard?.UnregisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
                    _blackboard?.UnregisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
                }
                _index = value;
                if (_index != null && old != null)//如果old等于null，说明现在是在被序列化，不应该重复绑定
                {
                    _blackboard?.RegisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
                    _blackboard?.RegisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
                    if (_blackboard != null && _blackboard.HasValue(_index))
                    {
                        Value = _blackboard.GetValue<T>(_index);
                    }
                }
            }
        }

        /// <summary>
        /// 是否不存在该索引
        /// </summary>
        public bool IsNotContainIndex => !GetValidIndex().Contains(Index);
        public bool IsNotContainDataSet => !GetValidDataSetIndex().Contains(DataSetName);

        protected override void OnValueUpdate()
        {
            if (_blackboard != null && _blackboard.HasValue(_index))
            {
                _value = _blackboard.GetValue<T>(_index);
            }
        }

        protected override object GetValue(string name)
        {
            return Value;
        }

        protected override void OnAttach()
        {
            _value = _blackboard.GetValue<T>(_index);
            _blackboard.RegisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
            _blackboard.RegisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
            //todo:处理当变量移除时的逻辑
            base.OnAttach();
        }

        protected void OnIndexChanged(IBlackboard.NameChangedEvent e)
        {
            _index = e.Name;
        }

        protected void OnValueChanged(IBlackboard.ValueChangeEvent e)
        {
            Value = (T)e.NewValue;
        }

        /// <summary>
        /// 获取存在的索引列表
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<string> GetValidIndex()
        {
            List<string> validIndex = new List<string>();
            var type = typeof(T);
            if (_dataSet != null)
            {
                foreach (var blackboardItem in _dataSet.GetVariables())
                {
                    if (blackboardItem.Value.TypeOfValue == type)
                    {
                        validIndex.Add(blackboardItem.Key);
                    }
                }
            }
            return validIndex;
        }
        protected IEnumerable<string> GetValidDataSetIndex()
        {
            return DataSetManager.Instance.ExistDataSets;
        }
    }
}
