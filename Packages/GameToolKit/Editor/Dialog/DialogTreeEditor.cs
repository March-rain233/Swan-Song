using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace GameToolKit.Dialog.Editor
{
    public class DialogTreeEditor : EditorWindow
    {
        public VisualTreeAsset VisualTree;
        public DialogTreeView GraphView;
        Label _filePath;
        ToolbarToggle _inspector;

        [MenuItem("GameToolKit/Dialog Tree Editor")]
        public static void ShowExample()
        {
            DialogTreeEditor wnd = GetWindow<DialogTreeEditor>();
            wnd.titleContent = new GUIContent("Dialog Tree Editor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            VisualTree.CloneTree(root);
            GraphView = root.Q<DialogTreeView>();
            _filePath = root.Q<Label>("filepath");
            _inspector = root.Q<ToolbarToggle>("inspector");
            root.Q<ToolbarButton>("save").clicked += GraphView.SaveChange;

            GraphView.Window = this;
            _inspector.RegisterValueChangedCallback(e=>GraphView.ShowInspector(e.newValue));
        }

        private void LoadTree(DialogTree tree)
        {
            _filePath.text = AssetDatabase.GetAssetPath(tree);
            GraphView.PopulateView(tree);
            GraphView.ShowInspector(_inspector.value);
        }

        private void OnSelectionChange()
        {
            DialogTree tree = Selection.activeObject as DialogTree;
            if (tree != null && !AssetDatabase.Contains(tree))
            {
                tree = null;
            }
            if (tree != null)
            {
                LoadTree(tree);
            }
        }
    }
}