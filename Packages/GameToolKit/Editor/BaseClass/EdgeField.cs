using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
namespace GameToolKit.Editor
{
    public class EdgeField : GraphElementField
    {
        private class Assist : SerializedScriptableObject
        {
            public SourceInfo info;
            [EnumToggleButtons]
            public SyncType syncType;
            public System.Action<SourceInfo, SyncType> callback;
        }
        [CustomEditor(typeof(Assist))]
        private class AssistEditor : Sirenix.OdinInspector.Editor.OdinEditor
        {
            Assist assist => target as Assist;
            bool foldout = true;
            public override void OnInspectorGUI()
            {
                SirenixEditorGUI.BeginBox();
                SirenixEditorGUI.BeginBoxHeader();
                foldout = SirenixEditorGUI.Foldout(foldout, $"{assist.info.SourceNode.Name}.{assist.info.SourceField}->{assist.info.TargetNode.Name}.{assist.info.TargetField}");
                SirenixEditorGUI.EndBoxHeader();
                if (foldout)
                {
                    var oldType = assist.syncType;
                    assist.syncType = (SyncType)SirenixEditorFields.EnumDropdown("Sync Type", assist.syncType);
                    if(oldType != assist.syncType)
                    {
                        assist.callback(assist.info, assist.syncType);
                    }
                }
                SirenixEditorGUI.EndBox();
            }
        }

        private SourceInfo _instance;
        public EdgeField(SourceInfo info, SyncType type, System.Action<SourceInfo, SyncType> callback)
        {
            _instance = info;
            Assist test = ScriptableObject.CreateInstance<Assist>();
            test.callback = callback;
            test.syncType = type;
            test.info = info;
            var editor = Sirenix.OdinInspector.Editor.OdinEditor.CreateEditor(test);
            IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            Add(container);
        }

        public override bool IsAssociatedWith(object obj)
        {
            return _instance.Equals(obj);
        }
    }
}
