using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEditor;

namespace GameToolKit.Editor
{
    /// <summary>
    /// 泛型选择器
    /// </summary>
    public class GenericSelectorDialog : OdinEditorWindow
    {
        /// <summary>
        /// 类型数据
        /// </summary>
        [Serializable]
        public struct TypeData
        {
            /// <summary>
            /// 类型
            /// </summary>
            [LabelText("$Name")]
            //[ValueDropdown("ValidTypes")] 当type数量太多时会发生卡顿，所以用默认的type字段
            [InfoBox("该类型不满足约束条件", InfoMessageType.Error, "IsValid")]
            public Type Type;
            /// <summary>
            /// 泛型参数名
            /// </summary>
            [HideInInspector]
            public string Name;
            /// <summary>
            /// 可使用的类型
            /// </summary>
            [ReadOnly, Searchable(FilterOptions = SearchFilterOptions.ValueToString, FuzzySearch = false, Recursive = false), EnableGUI]
            public IEnumerable<Type> ValidTypes;
            /// <summary>
            /// 当前类型是否合法
            /// </summary>
            public bool IsValid => Type != null && ValidTypes.Contains(Type);
            public static IEnumerable<Type> GetValidTypes(Type[] constraints)
            {
                if(constraints.Length == 0)
                {
                    return TypeCache.GetTypesDerivedFrom(typeof(object));
                }
                else
                {
                    var set = new HashSet<Type>(TypeCache.GetTypesDerivedFrom(constraints[0]));
                    for(int i = 1; i < constraints.Length; i++)
                    {
                        set.IntersectWith(TypeCache.GetTypesDerivedFrom(constraints[i]));
                    }
                    return set;
                }
            }
        }
        [ListDrawerSettings(IsReadOnly =true)]
        [OdinSerialize, NonSerialized]
        public TypeData[] TypeDatas;
        [HideInInspector]
        public Action Callback;

        [Button("OK")]
        public void OnWizardCreate()
        {
            Close();
            Callback?.Invoke();
        }

        public static void Display()
        {
            EditorWindow.CreateWindow<GenericSelectorDialog>().ShowModal();
        }
    }
}
