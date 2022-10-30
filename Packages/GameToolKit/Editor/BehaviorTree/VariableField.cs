using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameToolKit.Editor
{
    public class VariableField : GraphElementField
    {
        /// <summary>
        /// ������ʾ��
        /// </summary>
        public class Assist : SerializedScriptableObject
        {
            public BlackboardVariable Element;
            public string Name;
        }

        /// <summary>
        /// ������༭��
        /// </summary>
        private class AssistEditor : Sirenix.OdinInspector.Editor.OdinEditor
        {
            Assist assist => target as Assist;
            bool foldout = true;
            public override void OnInspectorGUI()
            {
                SirenixEditorGUI.BeginBox();
                SirenixEditorGUI.BeginBoxHeader();
                foldout = SirenixEditorGUI.Foldout(foldout, assist.Name);
                SirenixEditorGUI.EndBoxHeader();
                if (foldout)
                {
                    Tree.BeginDraw(true);
                    var property = Tree.GetPropertyAtPath("Element");
                    var children = property.Children;
                    foreach (var child in children)
                    {
                        if (child.Attributes.Where(a => a.GetType() == typeof(HideInGraphInspectorAttribute)).Count() > 0)
                        {
                            continue;
                        }
                        child.Draw();
                    }
                    Tree.EndDraw();
                }
                SirenixEditorGUI.EndBox();
            }
        }

        /// <summary>
        /// ������ʵ��
        /// </summary>
        public UnityEditor.Editor Inspector;

        /// <summary>
        /// ����������
        /// </summary>
        public IMGUIContainer GUIContainer;

        /// <summary>
        /// �󶨵Ķ���
        /// </summary>
        public BlackboardVariable Instance;

        /// <summary>
        /// ����ָ��������ֶ�
        /// </summary>
        /// <param name="element">����</param>
        /// <param name="Name">������ʾ������</param>
        public VariableField(BlackboardVariable element, string Name)
        {
            Instance = element;
            Assist assist = ScriptableObject.CreateInstance<Assist>();
            assist.Element = Instance;
            assist.Name = Name;
            Inspector = Sirenix.OdinInspector.Editor.OdinEditor.CreateEditor(assist, typeof(AssistEditor));
            GUIContainer = new IMGUIContainer(() => { Inspector.OnInspectorGUI(); });
            Add(GUIContainer);
        }

        public override bool IsAssociatedWith(object obj)
        {
            return Instance.Equals(obj);
        }
    }
}
