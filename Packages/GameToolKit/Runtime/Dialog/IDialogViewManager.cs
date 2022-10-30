using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话显示管理器接口
    /// </summary>
    public interface IDialogViewManager : IService
    {
        /// <summary>
        /// 获取对话框
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IDialogBox GetDialogBox(System.Type type);

        /// <summary>
        /// 获取选择框
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IOptionalView GetOptionalView(System.Type type);
    }
}
