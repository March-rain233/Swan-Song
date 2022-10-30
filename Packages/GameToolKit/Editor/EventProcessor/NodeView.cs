using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using System;
namespace GameToolKit.EventProcessor.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        /// <summary>
        /// 节点实例
        /// </summary>
        public Node Node;

        private Label _title;
        private TextField _input;
        public NodeView(Node node)
        {
            Node = node;

            _title = titleContainer.Q<Label>("title-label");
            _input = new TextField();
            titleContainer.Add(_input);
            _input.PlaceBehind(_title);
            _input.style.display = DisplayStyle.None;
            _input.RegisterCallback<FocusOutEvent>(e =>
            {
                _input.style.display = DisplayStyle.None;
                _title.style.display = DisplayStyle.Flex;
                ChangeName(_input.text);
            });

            if (node.Name == null)
                node.Name = node.GetType().Name;
            title = node.Name;
            name = node.Name;
            _input.value = node.Name;

            viewDataKey = node.Guid;

            style.left = node.ViewPosition.x;
            style.top = node.ViewPosition.y;
            //创建资源端口
            var list = node.GetValidPortDataList();
            foreach(var portData in list)
            {
                var tooltip = portData.PreferredType.Name;
                foreach(var t in portData.PortTypes)
                {
                    if(t != portData.PreferredType)
                    {
                        tooltip += $" {t.Name}";
                    }
                }
                if (portData.PortDirection == PortDirection.Input)
                {
                    var port = base.InstantiatePort(Orientation.Horizontal,
                        UnityEditor.Experimental.GraphView.Direction.Input,
                        Port.Capacity.Single,
                        portData.PreferredType);
                    port.portName = portData.NickName;
                    port.name = portData.Name;
                    port.tooltip = tooltip;
                    port.userData = portData.PortTypes;
                    inputContainer.Add(port);
                }
                else
                {
                    var port = base.InstantiatePort(Orientation.Horizontal,
                        UnityEditor.Experimental.GraphView.Direction.Output,
                        Port.Capacity.Multi,
                        portData.PreferredType);
                    port.portName = portData.NickName;
                    port.name = portData.Name;
                    port.tooltip = tooltip;
                    port.userData = portData.PortTypes;
                    outputContainer.Add(port);
                }
            }
            RefreshPorts();
            //设置外观
            titleContainer.style.backgroundColor = (node.GetType().GetCustomAttributes(typeof(NodeColorAttribute), true)[0] as NodeColorAttribute).Color;
            ColorUtility.TryParseHtmlString("#000000", out var temp);
            titleContainer.Q<Label>().style.color = temp;
        }

        /// <summary>
        /// 更改节点的名字
        /// </summary>
        public void ChangeName(string name)
        {
            Node.Name = name;
            this.name = name;
            this.title = name;
            _input.value = name;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.ViewPosition = new Vector2(newPos.xMin, newPos.yMin);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Rename", evt =>
            {
                _input.style.display = DisplayStyle.Flex;
                _title.style.display = DisplayStyle.None;
            });
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }
    }
}
