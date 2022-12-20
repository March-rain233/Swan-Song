using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// øÚº‹…Ë÷√
    /// </summary>
    [CreateAssetMenu(menuName = "Config/Frame Setting", fileName = "Frame Setting")]
    public class FrameSetting : ScriptableSingleton<FrameSetting>
    {
        [ValueDropdown("GetInitializer")]
        public System.Type Initializer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            ((ServiceInitializer)System.Activator.CreateInstance(Instance.Initializer)).Initialize();
        }
#if UNITY_EDITOR
        IEnumerable<System.Type> GetInitializer()
        {
            return UnityEditor.TypeCache.GetTypesDerivedFrom<ServiceInitializer>();
        }
#endif
    }
}
