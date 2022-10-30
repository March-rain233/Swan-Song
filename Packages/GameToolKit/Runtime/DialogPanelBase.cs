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

        public void Wait(DialogTree dialogTree)
        {
            _waitingList.Add(dialogTree);
        }

        protected override void OnInit()
        {
            ServiceFactory.Instance.GetService<EventManager>()
                .RegisterCallback<DialogEndEvent>(Close);
        }

        protected override void OnDispose()
        {
            ServiceFactory.Instance.GetService<EventManager>()
                .UnregisterCallback<DialogEndEvent>(Close);
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        void Close(DialogEndEvent context)
        {
            if (_waitingList.Remove(context.DialogTree))
            {
                ServiceFactory.Instance.GetService<PanelManager>().ClosePanel(this);
                if(_waitingList.Count <= 0)
                {
                    ServiceFactory.Instance.GetService<PanelManager>().ClosePanel(this);
                }
            }
        }
    }
}
