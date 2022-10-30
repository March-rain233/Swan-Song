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
        /// 辅助显示类
        /// </summary>
        public class Assist : SerializedScriptableObject
        {
            public BlackboardVariable Element;
            public string Name;
        }

        /// <summary>
        /// 辅助类编辑器
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
        /// 监视器实例
        /// </summary>
        public UnityEditor.Editor Inspector;

        /// <summary>
        /// 监视器容器
        /// </summary>
        public IMGUIContainer GUIContainer;

        /// <summary>
        /// 绑定的对象
        /// </summary>
        public BlackboardVariable Instance;

        /// <summary>
        /// 创建指定对象的字段
        /// </summary>
        /// <param name="element">对象</param>
        /// <param name="Name">对象显示的名字</param>
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
