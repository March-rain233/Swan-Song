using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի��ڵ����
    /// </summary>
    [NodeCategory("Dialog")]
    public abstract class DialogNodeBase : ProcessNode
    {
        /// <summary>
        /// �ٿص���ͼ����
        /// </summary>
        [ValueDropdown("GetValidViewType")]
        public Type ViewType;

        /// <summary>
        /// ��ȡ���ʵ���ͼ����
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<Type> GetValidViewType();
    }

    /// <summary>
    /// �Ի��ڵ�
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
        /// ��ȡѡ���б�
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<TextArgument> GetDialogList();

        /// <summary>
        /// ����Ի�
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
    /// ��Ի��ڵ�
    /// </summary>
    public class MultiDialogNode : DialogNode
    {
        /// <summary>
        /// �Ի�����
        /// </summary>
        [ListDrawerSettings(AddCopiesLastElement = true, ShowItemCount = true)]
        public List<TextArgument> Sentences = new List<TextArgument>();

        protected override IEnumerable<TextArgument> GetDialogList()
        {
            return Sentences;
        }
    }

    /// <summary>
    /// ���Ի��ڵ�
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
    /// ѡ��ѡ��ڵ�
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
    /// ѡ�����ݽڵ�
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
        /// ��ȡѡ���б�
        /// </summary>
        /// <returns></returns>
        protected internal abstract IEnumerable<OptionArgument> GetOptions();
    }

    /// <summary>
    /// ��ѡ�����ݽڵ�
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