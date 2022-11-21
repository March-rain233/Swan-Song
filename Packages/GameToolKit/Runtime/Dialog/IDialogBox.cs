using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话框接口
    /// </summary>
    public interface IDialogBox
    {
        /// <summary>
        /// 播放对话
        /// </summary>
        /// <param name="argument"></param>
        public void PlayDialog(TextArgument argument, Action onDialogEnd = null);

        /// <summary>
        /// 注册等待列表
        /// </summary>
        /// <param name="dialogTree"></param>
        public void Rigister(DialogTree dialogTree);

        /// <summary>
        /// 注销等待列表
        /// </summary>
        /// <param name="dialogTree"></param>
        public void Unrigister(DialogTree dialogTree);
    }
}
