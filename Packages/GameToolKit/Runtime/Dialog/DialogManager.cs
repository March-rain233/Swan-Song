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
    /// �Ի�ϵͳ������
    /// </summary>
    public class DialogManager : IService
    {
        /// <summary>
        /// �ȴ����еĶԻ�����
        /// </summary>
        [ShowInInspector, ReadOnly]
        public Queue<DialogTree> WaitQueue { get; private set; } = new Queue<DialogTree> ();

        /// <summary>
        /// �������еĶԻ��б�
        /// </summary>
        [ShowInInspector, ReadOnly]
        public List<DialogTree> RunningList { get; private set; } = new List<DialogTree> ();

        /// <summary>
        /// ���ŶԻ�
        /// </summary>
        /// <param name="name"></param>
        [Button]
        public void PlayDialog(DialogTree dialog)
        {
            WaitQueue.Enqueue(dialog);
            Refresh();
        }

        /// <summary>
        /// ˢ�����ݣ���鵱ǰ�߼�
        /// </summary>
        private void Refresh()
        {
            //�����ڶԻ��ڵȴ���ֱ�ӷ���
            if(WaitQueue.Count == 0)
            {
                return;
            }

            //������жԻ�
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

            //��ǰû�жԻ�������ʱ������Ի�
            if(RunningList.Count == 0)
            {
                addDialog();
            }

            //�����ǰ�ĶԻ�����˳��ģʽ�Ļ�����������Ĳ���ģʽ�Ի�
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
