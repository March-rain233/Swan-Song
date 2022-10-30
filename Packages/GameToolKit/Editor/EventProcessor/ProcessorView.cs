using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using GameToolKit.Editor;

namespace GameToolKit.EventProcessor.Editor
{
    public class ProcessorView : CustomGraphView<Node>
    {
        public new class UxmlFactory : UxmlFactory<ProcessorView, UxmlTraits> { }

        AutomaticProcessor _processor => Graph as AutomaticProcessor;

        const string _elementSetting = "Element Setting";
        const string _processorSetting = "Processor Setting";

        protected override GraphInspector CreateInspector()
        {
            return new GraphInspector(new string[] {_elementSetting, _processorSetting});
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
            _inspector.ClearTabAll();
            _inspector.AddToTab(new ProcessorField(_processor), _processorSetting);
        }
        protected override void ClearCurrentInspector()
        {
            _inspector.ClearTab(_elementSetting);
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_processor != null)
            {
                base.BuildContextualMenu(evt);
                if (evt.target is NodeView && (evt.target as NodeView).Node is EventSenderNode)
                {
                    var list = evt.menu.MenuItems();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var action = list[i] as DropdownMenuAction;
                        if (action != null && action.name == "Delete")
                        {
                            evt.menu.RemoveItemAt(i);
                            evt.menu.InsertAction(i, "Delete", (e) => { }, DropdownMenuAction.Status.Disabled);
                            break;
                        }
                    }
                }
            }
        }
    }
}

