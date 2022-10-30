using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话参数基类
    /// </summary>
    public abstract class DialogArgument
    {
    }
    /// <summary>
    /// 文本参数
    /// </summary>
    public class TextArgument : DialogArgument 
    {
        /// <summary>
        /// 正文
        /// </summary>
        [TextArea]
        public string Text;
    }
    /// <summary>
    /// 选择项参数
    /// </summary>
    public class OptionArgument : DialogArgument 
    {
        /// <summary>
        /// 正文
        /// </summary>
        [TextArea]
        public string Option;
        /// <summary>
        /// 该选项是否启用
        /// </summary>
        public bool IsEnable;
    }
}
