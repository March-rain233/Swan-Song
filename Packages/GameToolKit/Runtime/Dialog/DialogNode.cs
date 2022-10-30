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
    public class DialogNode : DialogNodeBase
    {
        /// <summary>
        /// �Ի�����
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
        /// ����Ի�
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
    /// ѡ��ѡ��ڵ�
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
    /// ѡ�����ݽڵ�
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