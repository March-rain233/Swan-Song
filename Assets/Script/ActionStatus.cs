using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ActionStatus
{
    /// <summary>
    /// 仍在等待行动中
    /// </summary>
    Waitting,
    /// <summary>
    /// 正在行动
    /// </summary>
    Running,
    /// <summary>
    /// 已死亡
    /// </summary>
    Dead,
    /// <summary>
    /// 该回合已行动完毕
    /// </summary>
    Rest
}