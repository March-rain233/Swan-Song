using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit
{
    /// <summary>
    /// UIϵͳ����
    /// </summary>
    [CreateAssetMenu(fileName = "UISetting", menuName = "Config/UISetting")]
    public class UISetting : ScriptableSingleton<UISetting>
    {
        /// <summary>
        /// Ԥ�����ֵ�
        /// </summary>
        public Dictionary<string, GameObject> PrefabsDic;
    }
}
