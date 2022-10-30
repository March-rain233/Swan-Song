using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using GameToolKit.Behavior.Tree;
using GameToolKit.Editor;
using GameToolKit.Utility;

namespace GameToolKit.Behavior.Tree.Editor
{
    /// <summary>
    /// 行为树图
    /// </summary>
    public class TreeView : CustomGraphView<Node>
    {
        public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits> { }

        class GraphLayoutAdapter : Utility.GraphLayoutAdapter
        {
            TreeView _view;
            public override IEnumerable<int> Nodes => _nodes;
            List<int> _nodes;
            public GraphLayoutAdapter(TreeView view)
            {
                _view = view;
                _nodes = new();
                for (int i = 0; i < view.nodes.Where(n=>((TreeNodeView)n)?.Node is ProcessNode).Count(); i++)
                {
                    _nodes.Add(i);
                }
            }

            public override IEnumerable<int> GetDescendant(int id)
            {
                return _view.nodes.Where(n => ((TreeNodeView)n)?.Node is ProcessNode)
                    .ElementAt(id).outputContainer.Q<Port>("Next").connections
                    .Select(e=>e.input.node).Distinct()
                    .Select(e=>GetID(e as TreeNodeView));
            }

            public override NodeData GetNodeData(int id)
            {
                NodeData data = new NodeData();
                var node = _view.nodes.Where(n => ((TreeNodeView)n)?.Node is ProcessNode).ElementAt(id);
                data.Position = node.GetPosition().position;
                data.Width = node.layout.width;
                data.Height = node.layout.height;
                return data;
            }

            public override IEnumerable<int> GetPrecursor(int id)
            {
                return _view.nodes.Where(n => ((TreeNodeView)n)?.Node is ProcessNode)
                    .ElementAt(id).inputContainer.Q<Port>("Pre").connections
                    .Select(e => e.input.node).Distinct()
                    .Select(e => GetID(e as TreeNodeView));
            }

            public override void SetNodeData(int id, NodeData data)
            {
                var node = _view.nodes.Where(n => ((TreeNodeView)n)?.Node is ProcessNode).ElementAt(id);
                node.SetPosition(new Rect(data.Position, Vector2.zero));
            }

            public int GetID(TreeNodeView nodeView)
            {
                var nodes = _view.nodes.Where((n) => ((TreeNodeView)n)?.Node is ProcessNode);
                return nodes.Select((n, i) => i).Where(i => nodes.ElementAt(i) == nodeView).FirstOrDefault();
            }
        }

        /// <summary>
        /// 变量定义域
        /// </summary>
        private enum Domain
        {
            Global,
            Prototype,
            Local
        }

        private Blackboard _blackboard;
        private BehaviorTree _tree => Graph as BehaviorTree;

        public override bool supportsWindowedBlackboard => true;

        public TreeView() : base()
        {
            RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            });
            RegisterCallback<DragPerformEvent>(e =>
            {
                Vector2 point = contentViewContainer.WorldToLocal(e.localMousePosition);
                foreach (BlackboardField field in _blackboard.selection)
                {
                    (var variableType, var domain, var dataset) = (ValueTuple<Type, Domain, string>)field.userData; ;
                    Type type;
                    if(domain == Domain.Global)
                    {
                        type = typeof(DataSetVariableNode<>);
                    }
                    else
                    {
                        type = typeof(VariableNode<>);
                    }
                    type = type.MakeGenericType(variableType.BaseType.GetGenericArguments()[0]);
                    var view = CreateNode(type);
                    view.SetPosition(new Rect(point, Vector2.zero));
                    type.GetProperty("DataSetName")?.SetValue(view.Node, dataset);
                    type.GetProperty("Index").SetValue(view.Node, field.text);
                    view.ChangeName(field.text);
                }
            });
        }

        protected override NodeSearchProviderBase GetSearchWindowProvider()
        {
            var provider = ScriptableObject.CreateInstance<NodeSearchProvider>();
            provider.Init(this, Window);
            return provider;
        }

        public override void PopulateView(CustomGraph<Node> graph)
        {
            base.PopulateView(graph);
            //为传入的树的边生成视图
            foreach (var node in graph.Nodes)
            {
                var view = FindNodeView(node);
                //连接行为后继
                if (node is ProcessNode)
                {
                    var children = ((ProcessNode)node).GetChildren();
                    if (children == null || children.Length == 0)
                    {
                        continue;
                    }
                    Array.ForEach(children, child =>
                    {
                        NodeView childView = FindNodeView(child);
                        Edge edge = view.outputContainer.Q<Port>("Next").ConnectTo(childView.inputContainer.Q<Port>("Prev"));
                        AddElement(edge);
                    });
                }
            }
            if(_blackboard != null)
            {
                ReleaseBlackboard(_blackboard);
            }
            _blackboard = GetBlackboard();
        }

        protected override NodeView CreateNodeView(BaseNode node)
        {
            var view = new TreeNodeView(node as Node);
            AddElement(view);
            return view;
        }

        /// <summary>
        /// 当图发生变化时
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        protected override GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            Func<ProcessNode, ProcessNode, bool> order = (a, b) =>
            {
                return a.ViewPosition.y < b.ViewPosition.y;
            };
            //当元素被移除
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    RemoveFromSelection(elem);
                    //移除点
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        if (nodeView.Node is not RootNode)
                        {
                            Graph.RemoveNode(nodeView.Node as Node);
                        }
                        else
                        {
                            CreateNodeView(nodeView.Node);
                        }
                    }
                    //移除边
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        if (edge.input.name == "Prev" && edge.output.name == "Next")
                        {
                            (parentView.Node as ProcessNode).RemoveChild(childView.Node as ProcessNode);
                        }
                        else
                        {
                            if (((SyncType)edge.userData & SyncType.Pull) != 0)
                                childView.Node.InputEdges.RemoveAll(
                                    e => e.SourceNode == parentView.Node
                                    && e.TargetNode == childView.Node
                                    && e.SourceField == edge.output.name
                                    && e.TargetField == edge.input.name
                                    );
                            if (((SyncType)edge.userData & SyncType.Push) != 0)
                                parentView.Node.OutputEdges.RemoveAll(
                                    e => e.SourceNode == parentView.Node
                                    && e.TargetNode == childView.Node
                                    && e.SourceField == edge.output.name
                                    && e.TargetField == edge.input.name
                                    );
                        }
                    }
                });
            }
            //当边被创建
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    edge.userData = SyncType.Pull;
                    if (edge.input.name == "Prev" && edge.output.name == "Next")
                    {
                        (parentView.Node as ProcessNode).AddChild(childView.Node as ProcessNode);
                        (parentView.Node as ProcessNode).OrderChildren(order);
                    }
                    else
                    {
                        childView.Node.InputEdges.Add(new SourceInfo(parentView.Node, childView.Node, edge.output.name, edge.input.name));
                    }
                });
            }
            if(graphViewChange.movedElements != null)
            {
                foreach(var element in graphViewChange.movedElements)
                {
                    switch (element)
                    {
                        case NodeView node:
                            if(node.Node is ProcessNode)
                            {
                                var parent = node.inputContainer.Q<Port>("Prev")?.connections;
                                if(parent != null && parent.Count() > 0)
                                {
                                    ((parent.First().output.node as NodeView).Node as ProcessNode).OrderChildren(order);
                                }
                            }
                            break;
                    }
                }
            }

            return graphViewChange;
        }

        /// <summary>
        /// 显示黑板
        /// </summary>
        /// <param name="visible"></param>
        public void ShowBlackboard(bool visible)
        {
            _blackboard.visible = visible && Graph != null;
        }

        public override Blackboard GetBlackboard()
        {
            Dictionary<Type, string> variableTypeList = new Dictionary<Type, string>();
            {
                var list = TypeCache.GetTypesDerivedFrom(typeof(GenericVariable<>));
                foreach (var type in list)
                {
                    variableTypeList.Add(type, type.BaseType.GenericTypeArguments[0].Name);
                }
            }
            var blackboard = new Blackboard(this);
            blackboard.title = "Behavior Tree";
            blackboard.subTitle = "Blackboard";
            blackboard.scrollable = true;
            var global = new Foldout() { text = "Global" };
            global.value = false;
            var prototype = new Foldout() { text = "Prototype" };
            var local = new Foldout() { text = "Local" };
            Func<string , string, Type, bool, Domain, BlackboardField> getVariable = (string dataset, string name, Type type, bool newItem, Domain domain) =>
            {
                var field = new BlackboardField();
                field.text = name;
                field.typeText = variableTypeList[type];
                field.userData = (Type: type, Domain: domain, DataSet: dataset);
                if (domain != Domain.Global)
                {
                    bool init = newItem;
                    string oldName = name;
                    field.RegisterCallback<FocusOutEvent>(e =>
                    {
                        if (string.IsNullOrEmpty(field.text) ||
                        (_tree.Blackboard.HasValue(field.text) && field.text != oldName))
                        {
                            EditorUtility.DisplayDialog("Warring", "Name should not be empty or a variable with the same name already exists", "OK");
                            if (init)
                            {
                                _blackboard.Remove(field);
                            }
                            else
                            {
                                field.text = oldName;
                            }
                            _blackboard.ReleaseMouse();
                        }
                        else
                        {
                            if (init)
                            {
                                _tree.Blackboard.AddLocalVariable(field.text, Activator.CreateInstance(type) as BlackboardVariable);
                                init = false;
                            }
                            else
                            {
                                _tree.Blackboard.RenameValue(oldName, field.text);
                            }
                            oldName = field.text;
                        }
                    });
                    field.RegisterCallback<ContextualMenuPopulateEvent>(e =>
                    {
                        e.menu.AppendAction("Delete", e =>
                        {
                            _tree.Blackboard.RemoveValue(oldName);
                            field.RemoveFromHierarchy();
                        });
                    });
                    if (newItem)
                    {
                        field.OpenTextEditor();
                    }
                }
                else
                {
                    field.RegisterCallback<ContextualMenuPopulateEvent>(e =>
                    {
                        var count = e.menu.MenuItems().Count;
                        for(int i = count - 1; i >= 0; i--)
                        {
                            e.menu.RemoveItemAt(i);
                        }
                    });
                }
                return field;
            };
            //创建全局变量
            foreach(var dataset in DataSetManager.Instance.ExistDataSets)
            {
                var fold = new Foldout() { text = dataset };
                fold.Q<Toggle>().style.marginLeft = 0;
                fold.value = false;
                global.Add(fold);
                foreach(var variable in DataSetManager.Instance.GetDataSet(dataset).GetVariables())
                {
                    fold.Add(getVariable(dataset, variable.Key, variable.Value.GetType(), false, Domain.Global));
                }
            }
            foreach (var variable in _tree.Blackboard.GetLocalVariables())
            {
                local.Add(getVariable(null, variable.Key, variable.Value.GetType(), false, Domain.Local));
            }
            foreach(var variable in _tree.Blackboard.GetPrototypeVariables())
            {
                prototype.Add(getVariable(null, variable.Key, variable.Value.GetType(), false, Domain.Prototype));
            }
            //变量更改定义域
            local.RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            });
            local.RegisterCallback<DragPerformEvent>(e =>
            {
                var selection = new List<ISelectable>(blackboard.selection);
                foreach (BlackboardField field in selection)
                {
                    (var type, var domain, var dataset) = (ValueTuple<Type, Domain, string>)field.userData;
                    if (domain == Domain.Prototype)
                    {
                        _tree.Blackboard.MoveToLocal(field.text);
                        field.userData = (type, Domain.Local, dataset);
                        prototype.Remove(field);
                        local.Add(field);
                    }
                }
                e.StopImmediatePropagation();
            });
            prototype.RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            });
            prototype.RegisterCallback<DragPerformEvent>(e =>
            {
                var selection = new List<ISelectable>(blackboard.selection);
                foreach (BlackboardField field in selection)
                {
                    (var type, var domain, var dataset) = (ValueTuple<Type, Domain, string>)field.userData;
                    if (domain == Domain.Local)
                    {
                        _tree.Blackboard.MoveToPrototype(field.text);
                        field.userData = (type, Domain.Prototype, dataset);
                        local.Remove(field);
                        prototype.Add(field);
                    }
                }
                e.StopImmediatePropagation();
            });
            blackboard.addItemRequested = (blackboard) =>
            {
                var menu = new GenericMenu();
                foreach (var item in variableTypeList)
                {
                    string name = item.Value;
                    menu.AddItem(new GUIContent(name), false, () => local.Add(getVariable(null, "", item.Key, true, Domain.Local)));
                }
                menu.ShowAsContext();
            };
            Add(blackboard);
            blackboard.Add(global);
            blackboard.Add(prototype);
            blackboard.Add(local);
            blackboard.visible = true;
            return blackboard;
        }

        public override void ReleaseBlackboard(Blackboard toRelease)
        {
            RemoveElement(toRelease);
            _blackboard = null;
        }

        /// <summary>
        /// 整理全图
        /// </summary>
        public void SortGraph()
        {
            if (_tree == null) return;
            var adap = new GraphLayoutAdapter(this);
            GraphLayoutUtility.TreeLayout(adap, 
                adap.GetID(FindNodeView((Graph as BehaviorTree).RootNode) as TreeNodeView));
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_tree != null)
            {
                base.BuildContextualMenu(evt);
                if(evt.target is TreeNodeView && (evt.target as TreeNodeView).Node is RootNode)
                {
                    var list = evt.menu.MenuItems();
                    for(int i = 0; i < list.Count; i++)
                    {
                        var action = list[i] as DropdownMenuAction;
                        if(action != null && action.name == "Delete")
                        {
                            evt.menu.RemoveItemAt(i);
                            evt.menu.InsertAction(i, "Delete", (e) => { }, DropdownMenuAction.Status.Disabled);
                            break;
                        }
                    }
                }
            }
        }

        protected override void OnAddField(ISelectable selectable)
        {
            switch (selectable)
            {
                case Edge edge:
                    if (edge.input.name != "Prev" || edge.output.name != "Next")
                    {
                        base.OnAddField(selectable);
                    }
                    return;
                case BlackboardField field:
                    (var type, var domain, var dataset) = (ValueTuple<Type, Domain, string>)field.userData;
                    if (domain == Domain.Global)
                    {
                        _inspector.AddToTab(new VariableField(DataSetManager.Instance.GetDataSet(dataset).GetVariable(field.text), dataset + '-' + field.text));
                    }
                    else
                    {
                        _inspector.AddToTab(new VariableField(_tree.Blackboard.GetVariable(field.text), field.text));
                    }
                    return;
                default:
                    base.OnAddField(selectable);
                    return;
            }
        }

        protected override void OnRemoveField(ISelectable selectable)
        {
            switch (selectable)
            {
                case Edge edge:
                    if (edge.input.name != "Prev" || edge.output.name != "Next")
                    {
                        base.OnRemoveField(selectable);
                    }
                    return;
                case BlackboardField field:
                    (var type, var domain, var dataset) = (ValueTuple<Type, Domain, string>)field.userData;
                    BlackboardVariable v;
                    if (domain == Domain.Global)
                    {
                        v = DataSetManager.Instance.GetDataSet(dataset).GetVariable(field.text);
                    }
                    else
                    {
                        v = _tree.Blackboard.GetVariable(field.text);
                    }
                    RemoveAssociatedFieldAll(v);
                    return;
                default:
                    base.OnRemoveField(selectable);
                    return;
            }
        }

        public void Search(string text)
        {
            ClearSelection();
            var list = nodes.ToList();
            foreach(var node in list)
            {
                if (node.name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                {
                    AddToSelection(node);
                }
            }
        }
    }
}