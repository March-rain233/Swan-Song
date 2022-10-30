using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace GameToolKit {

    /// <summary>
    /// µ¥ÀýSO»ùÀà
    /// </summary>
    /// <typeparam name="TSingleton"></typeparam>
    public abstract class ScriptableSingleton<TSingleton> : SerializedScriptableObject 
        where TSingleton : ScriptableSingleton<TSingleton>
    {
        static TSingleton _instance;

        public static TSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<TSingleton>($"{typeof(TSingleton).Name}");
                }
                if (_instance == null)
                {
#if UNITY_EDITOR
                    _instance = CreateInstance<TSingleton>();
                    UnityEditor.AssetDatabase.CreateAsset(_instance, $@"Assets/Resources/{typeof(TSingleton).Name}.asset");
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
#else
                    Debug.LogError($"{typeof(TSingleton)} file miss");
#endif
                }
                return _instance;
            }
        }
    }
}
