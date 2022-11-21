using GameToolKit.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// 对话框界面基类
    /// </summary>
    public abstract class DialogPanelBase : PanelBase, IDialogBox
    {
        /// <summary>
        /// 等待的对话树列表
        /// </summary>
        protected HashSet<DialogTree> _waitingList = new HashSet<DialogTree>();

        [Button]
        public abstract void PlayDialog(TextArgument argument, Action onDialogEnd = null);

        public void Rigister(DialogTree dialogTree)
        {
            _waitingList.Add(dialogTree);
        }

        public void Unrigister(DialogTree dialogTree)
        {
            _waitingList.Remove(dialogTree);
            if(_waitingList.Count == 0)
            {
                OnWaitingListEmpty();
            }
        }

        protected abstract void OnWaitingListEmpty();

        protected override void OnInit()
        {
            ServiceFactory.Instance.GetService<EventManager>()
                .RegisterCallback<DialogEndEvent>(DialogEndHandler);
        }

        protected override void OnDispose()
        {
            ServiceFactory.Instance.GetService<EventManager>()
                .UnregisterCallback<DialogEndEvent>(DialogEndHandler);
        }

        void DialogEndHandler(DialogEndEvent context)
        {
            Unrigister(context.DialogTree);
        }
    }
}
