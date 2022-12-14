using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;

public abstract class GuiderBase
{
    protected GuiderManager GuiderManager => ServiceFactory.Instance.GetService<GuiderManager>();

    /// <summary>
    /// 初始化
    /// </summary>
    public abstract void Init();
}
