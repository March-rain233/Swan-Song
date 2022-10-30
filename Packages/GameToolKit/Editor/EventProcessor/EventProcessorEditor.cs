using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using GameToolKit.Editor;

namespace GameToolKit.EventProcessor.Editor
{

    public class EventProcessorEditor : EditorWindow
    {
        public VisualTreeAsset VisualTreeAsset;
        public StyleSheet StyleSheet;

        public ProcessorView GraphView;
        private Label _filePath;
        private ListView _listView;

        [MenuItem("GameToolKit/Processor Editor")]
        public static void ShowExample()
        {
            EventProcessorEditor wnd = GetWindow<EventProcessorEditor>();
            wnd.titleContent = new GUIContent("Processor Editor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualTreeAsset.CloneTree(root);
            //root.styleSheets.Add(StyleSheet);
            root.Q<Button>("create").clicked += CreateAsset;
            root.Q<ToolbarToggle>("inspector").RegisterValueChangedCallback<bool>(e => GraphView.ShowInspector(e.newValue));
            _filePath = root.Q<Label>("file-path");
            GraphView = root.Q<ProcessorView>();
            _listView = root.Q<ListView>("processor-list");

            var list = new List<AutomaticProcessor>(Resources.FindObjectsOfTypeAll<AutomaticProcessor>());
            _listView.itemsSource = list;
            _listView.RefreshItems();
            _listView.onSelectionChange += e =>
            {
                var list = e as List<object>;
                if(list.Count < 0)
                {
                    return;
                }
                var processor = list[0] as AutomaticProcessor;
                LoadProcessor(processor);
            };

            GraphView.Window = this;
        }

        private void CreateAsset()
        {
            var dialog = CreateWindow<GenericSelectorDialog>();
            dialog.TypeDatas = new GenericSelectorDialog.TypeData[1] {new GenericSelectorDialog.TypeData() { Name = "1"} };
            //var dialog = EventTypeDialog.CreateWindow<EventTypeDialog>();
            //dialog.maxSize = dialog.minSize = new Vector2(300, 100);
            //dialog.EventType = typeof(NormalEvent);
            //dialog.FolderPath = "Assets/Resources";
            //dialog.Callback = (t, n) =>
            //{
            //    var asset = CreateInstance<EventProcessor>();
            //    asset.EventName = n;
            //    var type = typeof(EventSenderNode<>);
            //    type = type.MakeGenericType(t);
            //    var node = asset.CreateNode(type);
            //    asset.SenderNode = node as EventSenderNode;
            //    AssetDatabase.CreateAsset(asset, dialog.FolderPath + "/" + dialog.EventName + ".asset");
            //};
            ////如果模态窗口的bug能解决就改成模态窗口
            //dialog.ShowPopup();
        }

        private void LoadProcessor(AutomaticProcessor processor)
        {
            _filePath.text = AssetDatabase.GetAssetPath(processor);
            GraphView.PopulateView(processor);
            GraphView.ShowInspector(rootVisualElement.Q<ToolbarToggle>("inspector").value);
        }

        private void OnSelectionChange()
        {
            AutomaticProcessor processor = Selection.activeObject as AutomaticProcessor;
            if (processor != null && !AssetDatabase.Contains(processor))
            {
                processor = null;
            }
            if (processor != null)
            {
                var index = _listView.itemsSource.IndexOf(processor);
                _listView.selectedIndex = index;
            }
            _listView.RefreshItems();
        }
        
        public void Refresh()
        {
            _listView.RefreshItems();
            LoadProcessor(_listView.selectedItem as AutomaticProcessor);
        }

        private void OnProjectChange()
        {
            var select = _listView.selectedItem;
            var list = Resources.FindObjectsOfTypeAll<AutomaticProcessor>();
            _listView.itemsSource = list;
            _listView.RefreshItems();
            var index = _listView.itemsSource.IndexOf(select);
            if(index != -1)
            {
                _listView.selectedIndex = index;
            }
            else
            {
                try
                {
                    _listView.selectedIndex = 0;
                }
                catch (NullReferenceException)
                {

                }
            }
        }
    } 
}