using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;

/// <summary>
/// 系统初始化器
/// </summary>
public class SystemInitializer : GeneralInitializer
{
    public override void Initialize()
    {
        base.Initialize();
        ServiceFactory.Instance.Register<MapRenderer, MapRenderer>();
        ServiceFactory.Instance.Register<GameManager, GameManager>();
    }
}
