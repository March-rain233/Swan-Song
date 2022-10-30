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
    public class DialogNode : DialogNodeBase
    {
        /// <summary>
        /// 对话数据
        /// </summary>
        [ListDrawerSettings(AddCopiesLastElement = true, ShowItemCount = true)]
        public List<TextArgument> Sentences = new List<TextArgument>();
        protected override void OnPlay()
        {
            var dialog = ServiceFactory.Instance.GetService<IDialogViewManager>().GetDialogBox(ViewType);
            dialog.Wait(DialogTree);
            Output(0, dialog);
        }

        /// <summary>
        /// 输出对话
        /// </summary>
        /// <param name="i"></param>
        /// <param name="dialogBox"></param>
        protected void Output(int i, IDialogBox dialogBox)
        {
            if(i >= Sentences.Count)
            {
                Finish();
            }
            else
            {
                dialogBox.PlayDialog(Sentences[i], ()=>Output(i + 1, dialogBox));
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
    /// 选项选择节点
    /// </summary>
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.Multi)]
    public class OptionSelectorNode : DialogNodeBase
    {
        public OptionNode SelectedOption;
        protected override void OnPlay()
        {
            var Options = new List<OptionNode>();
            foreach(OptionNode option in Children)
            {
                if (option.Option.IsEnable)
                {
                    Options.Add(option);
                }
            }

            ServiceFactory.Instance.GetService<IDialogViewManager>().GetOptionalView(ViewType)
                .ShowOptions(Options.Select(node=>node.Option).ToList(), selected =>
                {
                    SelectedOption = Options[selected];
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
            SelectedOption.Play();
        }
    }

    /// <summary>
    /// 选项数据节点
    /// </summary>
    [NodeCategory("Dialog")]
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.Multi)]
    public class OptionNode : ProcessNode
    {
        [ShowInNodeExtension]
        public OptionArgument Option = new OptionArgument();
        protected override void OnPlay()
        {
            Finish();
        }
    }
}