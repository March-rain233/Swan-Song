using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի���ʾ�������ӿ�
    /// </summary>
    public interface IDialogViewManager : IService
    {
        /// <summary>
        /// ��ȡ�Ի���
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IDialogBox GetDialogBox(System.Type type);

        /// <summary>
        /// ��ȡѡ���
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IOptionalView GetOptionalView(System.Type type);
    }
}
