using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Sirenix.OdinInspector;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using GameToolKit.Editor;

namespace GameToolKit.EventProcessor.Editor
{
    public class ProcessorField : GraphElementField
    {
        private class AssistEditor : OdinEditor
        {
            AutomaticProcessor assist => target as AutomaticProcessor;
            public override void OnInspectorGUI()
            {
                SirenixEditorGUI.BeginBox();
                Tree.BeginDraw(true);
                var children = Tree.RootProperty.Children;
                foreach (var child in children)
                {
                    if (child.Attributes.Where(a => a.GetType() == typeof(HideInGraphInspectorAttribute)).Count() > 0)
                    {
                        continue;
                    }
                    if(child.NiceName == "Nodes" || child.NiceName == "Sender Node")
                    {
                        continue;
                    }
                    child.Draw();
                }
                Tree.EndDraw();
                if (GUILayout.Button(new GUIContent("Change EventType", EditorIcons.SettingsCog.Raw)))
                {
                    var dialog = EditorWindow.CreateWindow<EventTypeDialog>();
                    dialog.EventType = assist.EventType;
                    dialog.EventName = assist.EventName;
                    dialog.ShowFolderPath = false;
                    dialog.Callback = (t, n) =>
                    {
                        if(t != assist.EventType)
                        {
                            var type = typeof(EventSenderNode<>);
                            type = type.MakeGenericType(t);
                            var ori = assist.SenderNode;
                            var node = assist.CreateNode(type);
                            assist.SenderNode = node as EventSenderNode;
                            assist.RemoveNode(ori);
                        }
                        assist.EventName = n;
                        EventProcessorEditor.GetWindow<EventProcessorEditor>().Refresh();
                    };
                    dialog.ShowPopup();
                }
                SirenixEditorGUI.EndBox();
            }
        }
        UnityEditor.Editor _editor;
        AutomaticProcessor instance;
        public ProcessorField(AutomaticProcessor processor)
        {
            instance = processor;
            _editor = Sirenix.OdinInspector.Editor.OdinEditor.CreateEditor(processor, typeof(AssistEditor));
            IMGUIContainer container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(container);
        }

        public override bool IsAssociatedWith(object obj)
        {
            return System.Object.Equals(instance, obj);
        }
    }
}
