using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit
{
    /// <summary>
    /// ��������
    /// </summary>
    public abstract class SingletonBase<TSingleton> where TSingleton : SingletonBase<TSingleton>
    {
        /// <summary>
        /// �������ʵ�
        /// </summary>
        public static TSingleton Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = System.Activator.CreateInstance<TSingleton>();
                }
                return _instance;
            }
        }
        protected static TSingleton _instance;
    }
}
