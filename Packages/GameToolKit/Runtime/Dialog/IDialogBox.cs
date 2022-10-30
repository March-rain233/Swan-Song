using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի���ӿ�
    /// </summary>
    public interface IDialogBox
    {
        /// <summary>
        /// ���ŶԻ�
        /// </summary>
        /// <param name="argument"></param>
        public void PlayDialog(TextArgument argument, Action onDialogEnd = null);

        /// <summary>
        /// �ȴ��Ի����������ٹر�
        /// </summary>
        /// <param name="dialogTree"></param>
        public void Wait(DialogTree dialogTree);
    }
}
