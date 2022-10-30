using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;

namespace GameToolKit.Behavior.Tree.Editor
{
    [CustomEditor(typeof(BehaviorTreeRunner))]
    public class RunnerInspector : OdinEditor
    {
        BehaviorTreeRunner _behaviorTreeRunner => target as BehaviorTreeRunner;
        bool _foldout = true;
        public override void OnInspectorGUI()
        {
            GUIHelper.BeginDrawToNothing();
            base.OnInspectorGUI();
            GUIHelper.EndDrawToNothing();
            if (!EditorApplication.isPlaying)
            {
                var temp = SirenixEditorFields.UnityObjectField(new GUIContent("BTree"), _behaviorTreeRunner.ModelTree, typeof(BehaviorTree), false) as BehaviorTree;
                if(temp != _behaviorTreeRunner.ModelTree)
                {
                    _behaviorTreeRunner.ModelTree = temp;
                }
            }
            else
            {
                GUIHelper.PushGUIEnabled(false);
                EditorGUILayout.ObjectField(new GUIContent(), _behaviorTreeRunner.RunTree, typeof(BehaviorTreeRunner), true);
                GUIHelper.PopGUIEnabled();
            }
            if (!EditorApplication.isPlaying && _behaviorTreeRunner.ModelTree != null)
            {
                SirenixEditorGUI.BeginBox();
                GUIHelper.GetCurrentLayoutStyle().padding = new RectOffset(0, 0, 0, 0);
                _foldout = SirenixEditorGUI.Foldout(_foldout, "Variables");
                if (SirenixEditorGUI.BeginFadeGroup(this, _foldout))
                {
                    SirenixEditorGUI.HorizontalLineSeparator(SirenixGUIStyles.BorderColor, 1);
                    var local = Tree.GetPropertyAtPath("Variables");
                    ColorUtility.TryParseHtmlString("#ffffff", out var oddColor);
                    ColorUtility.TryParseHtmlString("#000000", out var evenColor);
                    int colorCounter = 0;
                    for (int i = local.Children.Count - 1; i >= 0; --i)
                    {                        
                        var child = local.Children[i];
                        var key = child.Children.Get("Key").ValueEntry.WeakSmartValue as string;
                        GUIHelper.PushColor(++colorCounter % 2 == 1 ? oddColor : evenColor);
                        EditorGUILayout.BeginHorizontal("box");
                        GUIHelper.GetCurrentLayoutStyle().margin = new RectOffset(0, 0, 0, 0);
                        GUIHelper.GetCurrentLayoutStyle().padding = new RectOffset(4, 4, 3, 3);
                        GUIHelper.PopColor();
                        EditorGUILayout.LabelField(key, GUILayout.MaxWidth(100));
                        var rect = EditorGUILayout.BeginVertical();
                        GUIHelper.PushLabelWidth(100);
                        foreach(var property in child.Children.Get("Value").Children)
                        {
                            property.Draw(GUIContent.none);
                        }
                        GUIHelper.PopLabelWidth();
                        EditorGUILayout.EndVertical();
                        //SirenixEditorGUI.DrawVerticalLineSeperator(rect.x - 3, rect.y - 2, rect.height + 4);
                        if(SirenixEditorGUI.IconButton(EditorIcons.X, 15, 15))
                        {
                            _behaviorTreeRunner.Variables.Remove(key);
                            _behaviorTreeRunner.ModelTree.Blackboard.UnregisterCallback<IBlackboard.ValueRemoveEvent>(key, _behaviorTreeRunner.RemoveItem);
                            _behaviorTreeRunner.ModelTree.Blackboard.UnregisterCallback<IBlackboard.NameChangedEvent>(key, _behaviorTreeRunner.RenameItem);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (!EditorApplication.isPlaying)
                    {
                        var modelTree = PropertyTree.Create(_behaviorTreeRunner.ModelTree);
                        var blackboard = modelTree.GetPropertyAtPath("Blackboard")
                            .Children.Get("_local");
                        for (int i = 0; i < blackboard.Children.Count; ++i)
                        {
                            var child = blackboard.Children[i];
                            string key = child.Children.Get("Key").ValueEntry.WeakSmartValue as string;
                            if (_behaviorTreeRunner.Variables.ContainsKey(key))
                            {
                                continue;
                            }
                            GUIHelper.PushColor(++colorCounter % 2 == 1 ? oddColor : evenColor);
                            EditorGUILayout.BeginHorizontal("box");
                            GUIHelper.GetCurrentLayoutStyle().margin = new RectOffset(0, 0, 0, 0);
                            GUIHelper.GetCurrentLayoutStyle().padding = new RectOffset(4, 4, 3, 3);
                            GUIHelper.PopColor();
                            EditorGUILayout.LabelField(key);
                            if(SirenixEditorGUI.IconButton(EditorIcons.Plus, 15, 15))
                            {
                                var variable = (child.Children.Get("Value").ValueEntry.WeakSmartValue as BlackboardVariable).Clone();
                                _behaviorTreeRunner.Variables.Add(key, variable);
                                _behaviorTreeRunner.ModelTree.Blackboard.RegisterCallback<IBlackboard.ValueRemoveEvent>(key, _behaviorTreeRunner.RemoveItem);
                                _behaviorTreeRunner.ModelTree.Blackboard.RegisterCallback<IBlackboard.NameChangedEvent>(key, _behaviorTreeRunner.RenameItem);

                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                SirenixEditorGUI.EndFadeGroup();
                SirenixEditorGUI.EndBox();
            }
        }
    }
}