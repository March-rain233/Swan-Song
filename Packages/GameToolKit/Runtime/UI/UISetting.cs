using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit
{
    /// <summary>
    /// UI系统设置
    /// </summary>
    [CreateAssetMenu(fileName = "UISetting", menuName = "Config/UISetting")]
    public class UISetting : ScriptableSingleton<UISetting>
    {
        /// <summary>
        /// 预制体字典
        /// </summary>
        public Dictionary<string, GameObject> PrefabsDic;
    }
}
