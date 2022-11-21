using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameToolKit.Editor
{
    public abstract class CustomGraphView<TNode> : GraphView where TNode : BaseNode
    {
        #region ͼ���������
        public override bool supportsWindowedBlackboard => false;
        protected override bool canCopySelection => false;
        protected override bool canDeleteSelection => true;
        protected override bool canPaste => false;
        protected override bool canCutSelection => false;
        public override bool canGrabFocus => true;
        protected override bool canDuplicateSelection => false;
        #endregion

        #region ��ͼ���
        /// <summary>
        /// ������
        /// </summary>
        protected GraphInspector _inspector;
        #endregion

        /// <summary>
        /// ��ǰ������ͼ��
        /// </summary>
        public CustomGraph<TNode> Graph { get; protected set; }

        /// <summary>
        /// ���ŵĴ���
        /// </summary>
        public EditorWindow Window;

        #region ͼ���ʼ��
        public CustomGraphView()
        {
            var background = new GridBackground();
            Insert(0, background);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.march_rain.gametoolkit/Editor/BaseClass/USS/CustomGraph.uss");
            styleSheets.Add(styleSheet);

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), GetSearchWindowProvider());

            _inspector = CreateInspector();
            _inspector.visible = false;
            Add(_inspector);
        }

        protected abstract NodeSearchProviderBase GetSearchWindowProvider();

        protected virtual GraphInspector CreateInspector()
        {
            return new GraphInspector();
        }
        #endregion

        #region ͼ�����
        /// <summary>
        /// ����������ѡ���tree�ı�ʱ�����¼�����ͼ
        /// </summary>
        /// <param name="graph"></param>
        public virtual void PopulateView(CustomGraph<TNode> graph)
        {
            Graph = graph;

            //ɾ����һ��������ͼ
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            //Ϊ���������ԭ�нڵ�������ͼ
            foreach (var node in graph.Nodes)
            {
                CreateNodeView(node);
            }

            //Ϊ��������ı�������ͼ
            foreach (var node in graph.Nodes)
            {
                var view = FindNodeView(node);
                //������Դ��
                foreach (var edge in node.InputEdges)
                {
                    var sourcePort = FindNodeView(edge.SourceNode).outputContainer.Q<Port>(edge.SourceField);
                    var targetPort = view.inputContainer.Q<Port>(edge.TargetField);
                    //�鿴���Ƿ��Ѵ���
                    Edge e = sourcePort.connections.FirstOrDefault(e => e.input == targetPort);
                    if (e == null)
                    {
                        e = targetPort.ConnectTo(sourcePort);
                        e.userData = SyncType.Pull;
                    }
                    else
                    {
                        e.userData = SyncType.Pull | SyncType.Push;
                    }
                    AddElement(e);
                }
                foreach (var edge in node.OutputEdges)
                {
                    var targetPort = FindNodeView(edge.TargetNode).inputContainer.Q<Port>(edge.TargetField);
                    var sourcePort = view.outputContainer.Q<Port>(edge.SourceField);
                    //�鿴���Ƿ��Ѵ���
                    Edge e = targetPort.connections.FirstOrDefault(e => e.output == sourcePort);
                    if(e == null)
                    {
                        e = sourcePort.ConnectTo(targetPort);
                        e.userData = SyncType.Push;
                    }
                    else
                    {
                        e.userData = SyncType.Pull | SyncType.Push;
                    }
                    AddElement(e);
                }
            }

            //�޸���ʾ����
            _inspector.title = graph.name;
        }

        /// <summary>
        /// ��ͼ�����仯ʱ
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        protected virtual GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            //��Ԫ�ر��Ƴ�
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    RemoveFromSelection(elem);
                    //�Ƴ���
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        Graph.RemoveNode(nodeView.Node as TNode);
                    }
                    //�Ƴ���
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
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
                });
            }
            //���߱�����
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    edge.userData = SyncType.Pull;
                    childView.Node.InputEdges.Add(new SourceInfo(parentView.Node, childView.Node, edge.output.name, edge.input.name));
                });
            }
            return graphViewChange;
        }

        /// <summary>
        /// ������
        /// </summary>
        public void ClearView()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            _inspector.ClearTabAll();
            ShowInspector(false);

            Graph = null;
        }

        /// <summary>
        /// ���ݴ���ڵ㴴���ڵ���ͼ
        /// </summary>
        /// <param name="node"></param>
        protected virtual NodeView CreateNodeView(BaseNode node)
        {
            NodeView nodeView = new NodeView(node);
            AddElement(nodeView);
            return nodeView;
        }

        /// <summary>
        /// ͼ��ˢ��
        /// </summary>
        public void Refresh()
        {
            if(Graph == null)
            {
                ClearView();
            }
            else
            {
                //todo ˢ�¹���
            }
        }
        #endregion

        #region ͼ�����
        /// <summary>
        /// �����ڵ�
        /// </summary>
        /// <param name="type"></param>
        public NodeView CreateNode(Type type)
        {
            TNode node = Graph.CreateNode(type);
            return CreateNodeView(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var typeList = startPort.userData as HashSet<Type>;
            bool isAcceptAll = startPort.direction == Direction.Input && typeList.Contains(typeof(object));
            var list = ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction
                && endPort.node != startPort.node
                && (isAcceptAll || (endPort.userData as HashSet<Type>).Overlaps(typeList)))
                .ToList();
            return list;
        }

        /// <summary>
        /// ����ָ���ߵ����ݴ��䷽ʽ
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="type"></param>
        protected void SetEdge(SourceInfo edge, SyncType type)
        {
            //���Ҷ�Ӧ�ı���ͼ
            var e = edges.ToList().Find(e => new SourceInfo((e.output.node as NodeView).Node,
                    (e.input.node as NodeView).Node,
                    e.output.portName,
                    e.input.portName) == edge);
            //ת��Ϊͼ����ʵ�ʵı�����
            var actual = new SourceInfo((e.output.node as NodeView).Node,
                    (e.input.node as NodeView).Node,
                    e.output.name,
                    e.input.name);
            if ((type & SyncType.Pull) != 0)
            {
                if (edge.TargetNode.InputEdges.Find(e => e == actual) == default)
                {
                    edge.TargetNode.InputEdges.Add(actual);
                }
            }
            else
            {
                var temp = edge.TargetNode.InputEdges.Find(e => e == actual);
                if (temp != default)
                {
                    edge.TargetNode.InputEdges.Remove(temp);
                }
            }
            if ((type & SyncType.Push) != 0)
            {
                if (edge.SourceNode.OutputEdges.Find(e => e == actual) == default)
                {
                    edge.SourceNode.OutputEdges.Add(actual);
                }
            }
            else
            {
                var temp = edge.SourceNode.OutputEdges.Find(e => e == actual);
                if (temp != default)
                {
                    edge.SourceNode.OutputEdges.Remove(temp);
                }
            }
            if (type == 0)
            {
                RemoveFromSelection(e);
                e.output.Disconnect(e);
                e.input.Disconnect(e);
                RemoveElement(e);
            }
            else
            {
                e.userData = type;
            }
        }

        public void SaveChange()
        {
            EditorUtility.SetDirty(Graph);
            AssetDatabase.SaveAssets();
        }
        #endregion
        
        #region ����
        /// <summary>
        /// ��ʾ������
        /// </summary>
        /// <param name="visible"></param>
        public void ShowInspector(bool visible)
        {
            _inspector.visible = visible && Graph != null;
        }

        /// <summary>
        /// ���Ҷ�Ӧ�ڵ����ͼ
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public NodeView FindNodeView(BaseNode node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (Graph != null)
            {
                base.BuildContextualMenu(evt);
            }
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            OnAddField(selectable);
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            ClearCurrentInspector();
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            OnRemoveField(selectable);
        }

        /// <summary>
        /// �����ǰ�������ϵ��ֶ�
        /// </summary>
        protected virtual void ClearCurrentInspector()
        {
            _inspector.ClearTab();
        }

        /// <summary>
        /// ������������ֶ�
        /// </summary>
        /// <param name="field"></param>
        protected virtual void OnAddField(ISelectable selectable)
        {
            switch (selectable)
            {
                case NodeView node:
                    _inspector.AddToTab(new NodeField(node.Node, node.name +
                        $"({node.Node.GetType().Name.Split('.').Last()})"));
                    return;
                case Edge edge:
                    _inspector.AddToTab(new EdgeField(
                        new SourceInfo((edge.output.node as NodeView).Node,
                        (edge.input.node as NodeView).Node,
                        edge.output.portName,
                        edge.input.portName),
                        (SyncType)edge.userData,
                        SetEdge));
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// �Ƴ���ֵ�������ֶ�
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnRemoveField(ISelectable selectable)
        {
            switch (selectable)
            {
                case NodeView node:
                    RemoveAssociatedFieldAll(node.Node);
                    return;
                case Edge edge:
                    RemoveAssociatedFieldAll(new SourceInfo((edge.output.node as NodeView).Node,
                        (edge.input.node as NodeView).Node,
                        edge.output.portName,
                        edge.input.portName));
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// �Ƴ����Ӧֵ������������ֶ�
        /// </summary>
        /// <param name="value"></param>
        protected void RemoveAssociatedFieldAll(object value)
        {
            if (_inspector.TryGetAssociatedFieldAll(value, out var list))
            {
                foreach (var field in list)
                {
                    _inspector.RemoveFromTab(field.Value, field.Key);
                }
            }
        }
        #endregion
    }
}