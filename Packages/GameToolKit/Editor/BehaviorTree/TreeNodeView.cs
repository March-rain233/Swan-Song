using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameToolKit.Editor;

namespace GameToolKit.Behavior.Tree.Editor
{
    public class TreeNodeView : NodeView
    {
        public TreeNodeView(Node node) : base(node)
        {
            //设置边框
            var border = this;
            border.style.paddingBottom =
                border.style.paddingLeft =
                border.style.paddingRight =
                border.style.paddingTop = 5;
            border.style.borderRightWidth =
                border.style.borderLeftWidth =
                border.style.borderTopWidth =
                border.style.borderBottomWidth = 2;
            border.style.borderBottomLeftRadius =
                border.style.borderBottomRightRadius =
                border.style.borderTopLeftRadius =
                border.style.borderTopRightRadius = 6;
            border.style.borderBottomColor =
                border.style.borderTopColor =
                border.style.borderRightColor =
                border.style.borderLeftColor = new Color(0, 0, 0, 0);
            //当节点状态改变时更改边框颜色
            node.OnColorChanged += color =>
                            {
                                border.style.borderBottomColor =
                                border.style.borderTopColor =
                                border.style.borderRightColor =
                                border.style.borderLeftColor =
                                color;
                            };

            //创建流程端口
            var nodeAttr = node.GetType().GetCustomAttributes(typeof(NodeAttribute), true)[0] as NodeAttribute;
            if(nodeAttr.InputPort == NodeAttribute.PortType.Single)
            {
                var port = base.InstantiatePort(Orientation.Horizontal, 
                    UnityEditor.Experimental.GraphView.Direction.Input, 
                    Port.Capacity.Single, 
                    typeof(ProcessNode));
                port.portName = "Prev";
                port.name = "Prev";
                port.userData = new HashSet<Type>() { port.portType };
                inputContainer.Add(port);
                port.SendToBack();
            }
            if(nodeAttr.OutputPort == NodeAttribute.PortType.Single)
            {
                var port = base.InstantiatePort(Orientation.Horizontal, 
                    UnityEditor.Experimental.GraphView.Direction.Output, 
                    Port.Capacity.Single, 
                    typeof(ProcessNode));
                port.portName = "Next";
                port.name = "Next";
                port.userData = new HashSet<Type>() { port.portType };
                outputContainer.Add(port);
                port.SendToBack();
            }
            else if(nodeAttr.OutputPort == NodeAttribute.PortType.Multi)
            {
                var port = base.InstantiatePort(Orientation.Horizontal, 
                    UnityEditor.Experimental.GraphView.Direction.Output, 
                    Port.Capacity.Multi, 
                    typeof(ProcessNode));
                port.portName = "Next";
                port.name = "Next";
                port.userData = new HashSet<Type>() { port.portType };
                outputContainer.Add(port);
                port.SendToBack();
            }
            RefreshPorts();
        }
    }
}

