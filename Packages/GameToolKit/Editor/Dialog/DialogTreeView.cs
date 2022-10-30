using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameToolKit.Editor;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace GameToolKit.Dialog.Editor
{
    public class DialogTreeView : CustomGraphView<Node>
    {
        public new class UxmlFactory : UxmlFactory<DialogTreeView, UxmlTraits> { }
        protected override NodeSearchProviderBase GetSearchWindowProvider()
        {
            var provider = ScriptableObject.CreateInstance<NodeSearchProvider>();
            provider.Init(this, Window);
            return provider;
        }

        protected override NodeView CreateNodeView(BaseNode node)
        {
            NodeView nodeView = new DialogNodeView(node);
            AddElement(nodeView);
            return nodeView;
        }
        protected override GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
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
                        if (nodeView.Node is not EntryNode && nodeView.Node is not ExitNode)
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
                            (parentView.Node as ProcessNode).Children.Remove(childView.Node as ProcessNode);
                            (childView.Node as ProcessNode).Parents.Remove(parentView.Node as ProcessNode);
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
                        (parentView.Node as ProcessNode).Children.Add(childView.Node as ProcessNode);
                        (childView.Node as ProcessNode).Parents.Add(parentView.Node as ProcessNode);
                    }
                    else
                    {
                        childView.Node.InputEdges.Add(new SourceInfo(parentView.Node, childView.Node, edge.output.name, edge.input.name));
                    }
                });
            }
            return graphViewChange;
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
                    var children = ((ProcessNode)node).Children;
                    if (children == null || children.Count == 0)
                    {
                        continue;
                    }
                    foreach(var child in children)
                    {
                        NodeView childView = FindNodeView(child);
                        Edge edge = view.outputContainer.Q<Port>("Next").ConnectTo(childView.inputContainer.Q<Port>("Prev"));
                        AddElement(edge);
                    }
                }
            }
        }
    }
}
