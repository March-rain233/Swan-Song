using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameToolKit
{
    /// <summary>
    /// ���ݼ�������
    /// </summary>
    [CreateAssetMenu(fileName = "Data Set Manager", menuName = "Data Set/Data Set Manager")]
    [ExecuteInEditMode]
    public class DataSetManager : ScriptableSingleton<DataSetManager>
    {
        [OdinSerialize]

        private Dictionary<string, DataSet> _dataSets = new Dictionary<string, DataSet>();
        /// <summary>
        /// ���ݼ������б�
        /// </summary>
        public IEnumerable<string> ExistDataSets => _dataSets.Keys;

        /// <summary>
        /// ������ݼ�
        /// </summary>
        public void AddDataSet(string name, DataSet dataSet)
        {
            _dataSets.Add(name, dataSet);
        }
        /// <summary>
        /// �Ƴ����ݼ�
        /// </summary>
        /// <param name="name"></param>
        public void RemoveDataSet(string name)
        {
            _dataSets.Remove(name);
        }
        /// <summary>
        /// ��ȡ���ݼ�
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