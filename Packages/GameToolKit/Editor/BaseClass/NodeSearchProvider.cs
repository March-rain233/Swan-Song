using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace GameToolKit.Editor
{
    public class NodeSearchProvider<TNode> : NodeSearchProviderBase 
        where TNode : BaseNode
    {
        CustomGraphView<TNode> _graphView;
        EditorWindow _window;
        public void Init(CustomGraphView<TNode> graphView, EditorWindow editor)
        {
            _graphView = graphView;
            _window = editor;
        }
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            var typeList = TypeCache.GetTypesDerivedFrom<TNode>();
            var groups = TypeCache.GetTypesWithAttribute<NodeCategoryAttribute>()
                .Where(t=>t.IsSubclassOf(typeof(TNode)));
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Node"), 0));
            foreach (var g in groups)
            {
                var attr = g.GetCustomAttributes(typeof(NodeCategoryAttribute), false)[0] as NodeCategoryAttribute;
                if (attr.Category == "NULL")
                    continue;
                int index = 0;
                var path = attr.Category.Split('/');
                for (int i = 0; i < path.Length; i++)
                {
                    var find = tree.FindIndex(item => item.content.text == path[i]);
                    if (find == -1)
                    {
                        tree.Insert(index + 1, new SearchTreeGroupEntry(new GUIContent(path[i]), i + 1));
                        find = index + 1;
                    }
                    index = find;
                }
            }
            foreach (var type in typeList)
            {
                if (type.IsAbstract)
                    continue;
                var category = (type.GetCustomAttributes(typeof(NodeCategoryAttribute), true)[0] as NodeCategoryAttribute).Category;
                if (category == "NULL")
                {
                    continue;
                }
                //获取所在的分组
                int level = category.Split('/').Length;
                category = category.Split('/')[level - 1];
                int index = tree.FindIndex((item) => item.content.text == category);
                if (index == -1)
                {
                    Debug.LogError("出现了未定义分类的节点");
                }
                else
                {
                    var attr = type.GetCustomAttributes(typeof(NodeNameAttribute), false);
                    string name;
                    if (attr.Length == 0)
                    {
                        name = type.Name;
                        if (type.IsGenericType)
                        {
                            name = name.Remove(name.Length - 2);
                        }
                    }
                    else
                    {
                        name = ((NodeNameAttribute)attr[0]).Name;
                    }
                    tree.Insert(
                        index + 1,
                        new SearchTreeEntry(new GUIContent("    " + name))
                        {
                            level = level + 1,
                            userData = type,
                        });
                }
            }
            return tree;
        }

        public override bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var type = SearchTreeEntry.userData as System.Type;
            if (type.IsGenericTypeDefinition)
            {
                var args = type.GetGenericArguments();
                var dialog = EditorWindow.CreateWindow<GenericSelectorDialog>();
                dialog.TypeDatas = new GenericSelectorDialog.TypeData[args.Length];
                for(int i = 0; i < args.Length; i++)
                {
                    dialog.TypeDatas[i].Name = args[i].Name;
                    dialog.TypeDatas[i].ValidTypes = GenericSelectorDialog.TypeData.
                        GetValidTypes(args[i].GetGenericArguments());
                }
                dialog.Callback = () =>
                {
                    var args = dialog.TypeDatas.Select(d => d.Type).ToArray();
                    var target = type.MakeGenericType(args);
                    CreateNode(target, context);
                };
                dialog.ShowPopup();
                return true;
            }
            CreateNode(type, context);
            return true;
        }

        private void CreateNode(System.Type type, SearchWindowContext context)
        {
            //将屏幕坐标转化到视图空间
            var windowRoot = _window.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(windowMousePosition);

            var node = _graphView.CreateNode(type);
            node.SetPosition(new Rect(graphMousePosition, new Vector2()));
        }
    }
}
