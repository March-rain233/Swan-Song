using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit.Editor
{
    /// <summary>
    /// 数据同步方式
    /// </summary>
    [Flags]
    public enum SyncType
    {
        /// <summary>
        /// 由目标节点拉取数据
        /// </summary>
        Pull = 1,
        /// <summary>
        /// 由数据源推送数据
        /// </summary>
        Push = Pull << 1,
    }
}
