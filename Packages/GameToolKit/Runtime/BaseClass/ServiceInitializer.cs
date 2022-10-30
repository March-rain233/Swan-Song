using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit {
    /// <summary>
    /// 服务初始化器
    /// </summary>
    /// <remarks>
    /// 用于设定服务的创建方法与创建时间
    /// </remarks>
    public abstract class ServiceInitializer
    {
        public abstract void Initialize();
    }
}
