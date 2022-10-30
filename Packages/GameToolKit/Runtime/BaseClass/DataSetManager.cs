using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameToolKit
{
    /// <summary>
    /// 数据集管理器
    /// </summary>
    [CreateAssetMenu(fileName = "Data Set Manager", menuName = "Data Set/Data Set Manager")]
    [ExecuteInEditMode]
    public class DataSetManager : ScriptableSingleton<DataSetManager>
    {
        [OdinSerialize]

        private Dictionary<string, DataSet> _dataSets = new Dictionary<string, DataSet>();
        /// <summary>
        /// 数据集名称列表
        /// </summary>
        public IEnumerable<string> ExistDataSets => _dataSets.Keys;

        /// <summary>
        /// 添加数据集
        /// </summary>
        public void AddDataSet(string name, DataSet dataSet)
        {
            _dataSets.Add(name, dataSet);
        }
        /// <summary>
        /// 移除数据集
        /// </summary>
        /// <param name="name"></param>
        public void RemoveDataSet(string name)
        {
            _dataSets.Remove(name);
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public bool TryGetDataSet(string name, out DataSet dataSet)
        {
            return _dataSets.TryGetValue(name, out dataSet);
        }
        public DataSet GetDataSet(string name)
        {
            return _dataSets[name];
        }
    }
}