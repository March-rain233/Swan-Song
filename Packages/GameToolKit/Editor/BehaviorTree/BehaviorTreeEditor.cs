using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
namespace GameToolKit.Behavior.Tree.Editor
{
    /// <summary>
    /// ÐÐÎªÊ÷±à¼­Æ÷
    /// </summary>
    public class BehaviorTreeEditor : EditorWindow
    {
        public VisualTreeAsset VisualTreeAsset;
        public StyleSheet StyleSheet;

        public TreeView TreeView;
        private Label _filename;

        [MenuItem("GameToolKit/BTree Editor")]
        public static void ShowMenu()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BTree Editor");
        }
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualTreeAsset.CloneTree(root);
            root.styleSheets.Add(StyleSheet);

            TreeView = root.Q<TreeView>();
            _filename = root.Q<Label>("filename");
            root.Q("blackboard").RegisterCallback((ChangeEvent<bool> e) =>
            {
                TreeView.ShowBlackboard(e.newValue);
            });
            root.Q("inspector").RegisterCallback((ChangeEvent<bool> e) =>
            {
                TreeView.ShowInspector(e.newValue);
            });
            root.Q<ToolbarButton>("sort").clicked += TreeView.SortGraph;
            root.Q<ToolbarButton>("save").clicked += TreeView.SaveChange;
            root.Q<ToolbarSearchField>("search").RegisterCallback((InputEvent e) =>
            {
                TreeView.Search(e.newData);
            });

            TreeView.Window = this;
        }

        private void LoadTree(BehaviorTree tree)
        {
            _filename.text = AssetDatabase.GetAssetPath(tree);
            TreeView.PopulateView(tree);
            TreeView.ShowBlackboard(rootVisualElement.Q<ToolbarToggle>("blackboard").value);
            TreeView.ShowInspector(rootVisualElement.Q<ToolbarToggle>("inspector").value);
        }
        
        private void OnSelectionChange()
        {
            BehaviorTree tree = Selection.activeObject as BehaviorTree;
            if (tree != null && !AssetDatabase.Contains(tree))
            {
                tree = null;
            }
            else if(Selection.gameObjects.Length > 0 && 
                Selection.gameObjects[0].TryGetComponent<BehaviorTreeRunner>(out var runner))
            {
                if (EditorApplication.isPlaying)
                {
                    tree = runner.RunTree;
                }
                else
                {
                    tree = runner.ModelTree;
                }
            }
            if(tree != null)
            {
                LoadTree(tree);
            }
        }

    }
}