using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի��¼�
    /// </summary>
    /// <remarks>
    /// ͨ�����͸��¼���֪ͨ�Ի�ϵͳ���ŶԻ�
    /// </remarks>
    public abstract class DialogEvent : EventBase
    {
        public DialogTree DialogTree;
    }

    /// <summary>
    /// �Ի������¼�
    /// </summary>
    public class DialogEndEvent : DialogEvent
    {

    }

    /// <summary>
    /// �Ի������¼�
    /// </summary>
    public class DialogBeginEvent : DialogEvent
    {
        
    }
}