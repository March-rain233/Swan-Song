using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit.Editor;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog.Editor
{
    public class DialogNodeView : NodeView
    {
        public DialogNodeView(BaseNode node) : base(node)
        {
            //创建流程端口
            var nodeAttr = node.GetType().GetCustomAttributes(typeof(NodeAttribute), true)[0] as NodeAttribute;
            if (nodeAttr.InputPort == NodeAttribute.PortType.Single)
            {
                var port = base.InstantiatePort(Orientation.Horizontal,
                    UnityEditor.Experimental.GraphView.Direction.Input,
                    Port.Capacity.Single,
                    typeof(ProcessNode));
                port.portName = "Prev";
                port.name = "Prev";
                port.userData = new HashSet<Type>() { port.portType };
                inputContainer.Add(port);
                port.BringToFront();
            }
            else if (nodeAttr.InputPort == NodeAttribute.PortType.Multi)
            {
                var port = base.InstantiatePort(Orientation.Horizontal,
                    UnityEditor.Experimental.GraphView.Direction.Input,
                    Port.Capacity.Multi,
                    typeof(ProcessNode));
                port.portName = "Prev";
                port.name = "Prev";
                port.userData = new HashSet<Type>() { port.portType };
                inputContainer.Add(port);
                port.BringToFront();
            }
            if (nodeAttr.OutputPort == NodeAttribute.PortType.Single)
            {
                var port = base.InstantiatePort(Orientation.Horizontal,
                    UnityEditor.Experimental.GraphView.Direction.Output,
                    Port.Capacity.Single,
                    typeof(ProcessNode));
                port.portName = "Next";
                port.name = "Next";
                port.userData = new HashSet<Type>() { port.portType };
                outputContainer.Add(port);
                port.BringToFront();
            }
            else if (nodeAttr.OutputPort == NodeAttribute.PortType.Multi)
            {
                var port = base.InstantiatePort(Orientation.Horizontal,
                    UnityEditor.Experimental.GraphView.Direction.Output,
                    Port.Capacity.Multi,
                    typeof(ProcessNode));
                port.portName = "Next";
                port.name = "Next";
                port.userData = new HashSet<Type>() { port.portType };
                outputContainer.Add(port);
                port.BringToFront();
            }
        }
    }
}