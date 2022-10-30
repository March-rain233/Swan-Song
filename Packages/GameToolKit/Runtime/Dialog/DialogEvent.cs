using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话事件
    /// </summary>
    /// <remarks>
    /// 通过发送该事件来通知对话系统播放对话
    /// </remarks>
    public abstract class DialogEvent : EventBase
    {
        public DialogTree DialogTree;
    }

    /// <summary>
    /// 对话结束事件
    /// </summary>
    public class DialogEndEvent : DialogEvent
    {

    }

    /// <summary>
    /// 对话结束事件
    /// </summary>
    public class DialogBeginEvent : DialogEvent
    {
        
    }
}