using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի���������
    /// </summary>
    public abstract class DialogArgument
    {
    }
    /// <summary>
    /// �ı�����
    /// </summary>
    public class TextArgument : DialogArgument 
    {
        /// <summary>
        /// ����
        /// </summary>
        [TextArea]
        public string Text;
    }
    /// <summary>
    /// ѡ�������
    /// </summary>
    public class OptionArgument : DialogArgument 
    {
        /// <summary>
        /// ����
        /// </summary>
        [TextArea]
        public string Option;
        /// <summary>
        /// ��ѡ���Ƿ�����
        /// </summary>
        public bool IsEnable;
    }
}
