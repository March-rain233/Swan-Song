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
    /// ����ѡ����
    /// </summary>
    public class GenericSelectorDialog : OdinEditorWindow
    {
        /// <summary>
        /// ��������
        /// </summary>
        [Serializable]
        public struct TypeData
        {
            /// <summary>
            /// ����
            /// </summary>
            [LabelText("$Name")]
            //[ValueDropdown("ValidTypes")] ��type����̫��ʱ�ᷢ�����٣�������Ĭ�ϵ�type�ֶ�
            [InfoBox("�����Ͳ�����Լ������", InfoMessageType.Error, "IsValid")]
            public Type Type;
            /// <summary>
            /// ���Ͳ�����
            /// </summary>
            [HideInInspector]
            public string Name;
            /// <summary>
            /// ��ʹ�õ�����
            /// </summary>
            [ReadOnly, Searchable(FilterOptions = SearchFilterOptions.ValueToString, FuzzySearch = false, Recursive = false), EnableGUI]
            public IEnumerable<Type> ValidTypes;
            /// <summary>
            /// ��ǰ�����Ƿ�Ϸ�
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
