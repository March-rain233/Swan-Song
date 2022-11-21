using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话节点基类
    /// </summary>
    [NodeCategory("Dialog")]
    public abstract class DialogNodeBase : ProcessNode
    {
        /// <summary>
        /// 操控的视图类型
        /// </summary>
        [ValueDropdown("GetValidViewType")]
        public Type ViewType;

        /// <summary>
        /// 获取合适的视图类型
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<Type> GetValidViewType();
    }

    /// <summary>
    /// 对话节点
    /// </summary>
    [NodeCategory("Dialog/BodyText")]
    public abstract class DialogNode : DialogNodeBase
    {
        public bool IsReleaseDialogOnEnd = false;
        protected override void OnPlay()
        {
            var dialog = ServiceFactory.Instance.GetService<IDialogViewManager>().GetDialogBox(ViewType);
            dialog.Rigister(DialogTree);
            Output(0, GetDialogList(),dialog);
        }

        /// <summary>
        /// 获取选项列表
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<TextArgument> GetDialogList();

        /// <summary>
        /// 输出对话
        /// </summary>
        /// <param name="i"></param>
        /// <param name="dialogBox"></param>
        protected void Output(int i, IEnumerable<TextArgument> list, IDialogBox dialogBox)
        {
            if (i >= list.Count())
            {
                if (IsReleaseDialogOnEnd)
                {
                    dialogBox.Unrigister(DialogTree);
                }
                Finish();
            }
            else
            {
                dialogBox.PlayDialog(list.ElementAt(i), ()=>Output(i + 1, list, dialogBox));
            }
        }
        protected override IEnumerable<Type> GetValidViewType()
        {
#if UNITY_EDITOR
            return UnityEditor.TypeCache.GetTypesDerivedFrom(typeof(IDialogBox)).Where(t => !t.IsGenericType && !t.IsAbstract);
#else
            return null;
#endif
        }
    }

    /// <summary>
    /// 多对话节点
    /// </summary>
    public class MultiDialogNode : DialogNode
    {
        /// <summary>
        /// 对话数据
        /// </summary>
        [ListDrawerSettings(AddCopiesLastElement = true, ShowItemCount = true)]
        public List<TextArgument> Sentences = new List<TextArgument>();

        protected override IEnumerable<TextArgument> GetDialogList()
        {
            return Sentences;
        }
    }

    /// <summary>
    /// 单对话节点
    /// </summary>
    public class SingleDialogNode : DialogNode
    {
        [Port("TextArgument", PortDirection.Input)]
        public TextArgument Dialog;
        protected override IEnumerable<TextArgument> GetDialogList()
        {
            return new List<TextArgument> { Dialog };
        }
    }

    /// <summary>
    /// 选项选择节点
    /// </summary>
    [NodeCategory("Dialog/Option")]
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.Multi)]
    public class OptionSelectorNode : DialogNodeBase
    {
        [Port("SelectedOptionIndex", PortDirection.Output)]
        public int SelectedOptionIndex;

        [Port("SelectedOptionNode", PortDirection.Output)]
        public OptionNode SelectedOptionNode;

        protected override void OnInit()
        {
            base.OnInit();
            SelectedOptionIndex = -1;
            SelectedOptionNode = null;
        }

        protected override void OnPlay()
        {
            var table = new List<(int left, OptionNode node)>();
            var options = new List<OptionArgument>();
            foreach(var node in Children.OfType<OptionNode>())
            {
                var list = node.GetOptions();
                table.Add((options.Count, node));
                options.AddRange(list);
            }

            ServiceFactory.Instance.GetService<IDialogViewManager>().GetOptionalView(ViewType)
                .ShowOptions(options, selected =>
                {
                    var item = table.FindLast(p => p.left <= selected);
                    SelectedOptionIndex = selected;
                    SelectedOptionNode = item.node;
                    SelectedOptionNode.SelectedOptionIndex = selected - item.left;
                    Finish();
                });
        }
        protected override IEnumerable<Type> GetValidViewType()
        {
#if UNITY_EDITOR
            return UnityEditor.TypeCache.GetTypesDerivedFrom(typeof(IOptionalView)).Where(t => !t.IsGenericType && !t.IsAbstract);
#else
            return null;
#endif
        }
        protected override void RunSubsequentNode()
        {
            SelectedOptionNode.Play();
        }
    }

    /// <summary>
    /// 选项数据节点
    /// </summary>
    [NodeCategory("Dialog/Option")]
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.Multi)]
    public abstract class OptionNode : ProcessNode
    {
        [Port("SelectedOptionIndex", PortDirection.Output)]
        public int SelectedOptionIndex;

        protected override void OnInit()
        {
            base.OnInit();
            SelectedOptionIndex = -1;
        }

        protected override void OnPlay()
        {
            Finish();
        }

        /// <summary>
        /// 获取选项列表
        /// </summary>
        /// <returns></returns>
        protected internal abstract IEnumerable<OptionArgument> GetOptions();
    }

    /// <summary>
    /// 单选项数据节点
    /// </summary>
    public class SingleOptionNode : OptionNode
    {
        [Port("OptionArgument", PortDirection.Input)]
        public OptionArgument Option;
        protected internal override IEnumerable<OptionArgument> GetOptions()
        {
            return new List<OptionArgument>() { Option };
        }
    }
}