using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话系统管理器
    /// </summary>
    public class DialogManager : IService
    {
        /// <summary>
        /// 等待运行的对话队列
        /// </summary>
        [ShowInInspector, ReadOnly]
        public Queue<DialogTree> WaitQueue { get; private set; } = new Queue<DialogTree> ();

        /// <summary>
        /// 正在运行的对话列表
        /// </summary>
        [ShowInInspector, ReadOnly]
        public List<DialogTree> RunningList { get; private set; } = new List<DialogTree> ();

        /// <summary>
        /// 播放对话
        /// </summary>
        /// <param name="name"></param>
        [Button]
        public void PlayDialog(DialogTree dialog)
        {
            WaitQueue.Enqueue(dialog);
            Refresh();
        }

        /// <summary>
        /// 刷新数据，检查当前逻辑
        /// </summary>
        private void Refresh()
        {
            //不存在对话在等待，直接返回
            if(WaitQueue.Count == 0)
            {
                return;
            }

            //添加运行对话
            Action addDialog = () =>
            {
                if(WaitQueue.Count == 0)
                {
                    return;
                }

                var dialog = WaitQueue.Dequeue();

                Action onDialogEnd = null;
                onDialogEnd = () =>
                {
                    dialog.OnDialogEnd -= onDialogEnd;
                    RunningList.Remove(dialog);
                    ServiceFactory.Instance.GetService<EventManager>().Broadcast(new DialogEndEvent()
                    {
                        DialogTree = dialog,
                    });
                    Refresh();
                };

                dialog.OnDialogEnd += onDialogEnd;
                RunningList.Add(dialog);
                ServiceFactory.Instance.GetService<EventManager>().Broadcast(new DialogBeginEvent()
                {
                    DialogTree = dialog,
                });
                dialog.Play();
            };

            //当前没有对话在运行时，加入对话
            if(RunningList.Count == 0)
            {
                addDialog();
            }

            //如果当前的对话不是顺序模式的话，加入后续的并发模式对话
            if(RunningList.Count != 0 && RunningList[0].Mode != DialogTree.DialogMode.Sequential)
            {
                while (WaitQueue.Peek().Mode != DialogTree.DialogMode.Sequential)
                {
                    addDialog();
                }
            }
        }
        
        void IService.Init()
        {
        }
    }
}
