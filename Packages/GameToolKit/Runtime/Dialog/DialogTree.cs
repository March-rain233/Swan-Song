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
            /// 并发执行
            /// </summary>
            /// <remarks>
            /// 该对话可与其他并发对话共同执行
            /// </remarks>
            Concurrent,
            /// <summary>
            /// 顺序执行
            /// </summary>
            /// <remarks>
            /// 该对话只有等待其他所有对话执行完毕后才会开始执行，
            /// 且后续对话都要等到该对话执行完毕才可执行
            /// </remarks>
            Sequential
        }
        /// <summary>
        /// 运行模式
        /// </summary>
        public DialogMode Mode = DialogMode.Sequential;

        /// <summary>
        /// 入口节点
        /// </summary>
        public EntryNode EntryNode;

        /// <summary>
        /// 终止节点
        /// </summary>
        public ExitNode ExitNode;

        /// <summary>
        /// 当对话结束
        /// </summary>
        public event Action OnDialogEnd;

        private void Reset()
        {
            EntryNode = CreateNode(typeof(EntryNode)) as EntryNode;
            ExitNode = CreateNode(typeof(ExitNode)) as ExitNode;
        }

        /// <summary>
        /// 开始对话
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
        /// 结束对话
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
            //移除连接
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
