using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit.Editor
{
    /// <summary>
    /// ����ͬ����ʽ
    /// </summary>
    [Flags]
    public enum SyncType
    {
        /// <summary>
        /// ��Ŀ��ڵ���ȡ����
        /// </summary>
        Pull = 1,
        /// <summary>
        /// ������Դ��������
        /// </summary>
        Push = Pull << 1,
    }
}
