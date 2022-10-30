using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameToolKit.Behavior.Tree {
    [NodeCategory("NULL")]
    public class DataSetVariableNode<T> : VariableNode<T>
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
                if(old == value)
                {
                    return;
                }
                else
                {
                    if(old != null)
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
        protected override IBlackboard _blackboard => _dataSet;
        [OdinSerialize, HideInInspector]
        private DataSet _dataSet;

        public bool IsNotContainDataSet => !GetValidDataSetIndex().Contains(DataSetName);

        protected override IEnumerable<string> GetValidIndex()
        {
            List<string> validIndex = new List<string>();
            var type = typeof(T);
            if(_dataSet != null)
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

        protected virtual IEnumerable<string> GetValidDataSetIndex()
        {
            return DataSetManager.Instance.ExistDataSets;
        }
    }
}
