using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

namespace GameToolKit.Dialog
{
    [CreateAssetMenu(fileName = "DialogTree", menuName = "Dialog/Dialog Tree")]
    public class DialogTree : CustomGraph<Node>
    {
        public enum DialogMode
        {
            /// <summary>
            /// ����ִ��
            /// </summary>
            /// <remarks>
            /// �öԻ��������������Ի���ִͬ��
            /// </remarks>
            Concurrent,
            /// <summary>
            /// ˳��ִ��
            /// </summary>
            /// <remarks>
            /// �öԻ�ֻ�еȴ��������жԻ�ִ����Ϻ�ŻῪʼִ�У�
            /// �Һ����Ի���Ҫ�ȵ��öԻ�ִ����ϲſ�ִ��
            /// </remarks>
            Sequential
        }
        /// <summary>
        /// ����ģʽ
        /// </summary>
        public DialogMode Mode = DialogMode.Sequential;

        /// <summary>
        /// ��ڽڵ�
        /// </summary>
        public EntryNode EntryNode;

        /// <summary>
        /// ��ֹ�ڵ�
        /// </summary>
        public ExitNode ExitNode;

        /// <summary>
        /// ���Ի�����
        /// </summary>
        public event Action OnDialogEnd;

        private void Reset()
        {
            EntryNode = CreateNode(typeof(EntryNode)) as EntryNode;
            ExitNode = CreateNode(typeof(ExitNode)) as ExitNode;
        }

        /// <summary>
        /// ��ʼ�Ի�
        /// </summary>
        public void Play()
        {
            ServiceFactory.Instance.GetService<EventManager>().Broadcast(new DialogBeginEvent()
            {
                DialogTree = this
            });
            foreach(var node in Nodes)
            {
                node.Init();
            }
            EntryNode.Play();
        }

        /// <summary>
        /// �����Ի�
        /// </summary>
        public void Finish()
        {
            OnDialogEnd?.Invoke();
        }

        public override Node CreateNode(Type type)
        {
            var node = base.CreateNode(type);
            node.DialogTree = this;
            return node;
        }

        public override void RemoveNode(Node node)
        {
            base.RemoveNode(node);
            //�Ƴ�����
            var n = node as ProcessNode;
            if (n != null)
            {
                foreach(ProcessNode processNode in Nodes.Where(n=>n is ProcessNode))
                {
                    processNode.Parents.Remove(n);
                    processNode.Children.Remove(n);
                }
            }
        }
    }
}
