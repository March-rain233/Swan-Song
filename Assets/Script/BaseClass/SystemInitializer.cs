using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;

/// <summary>
/// ϵͳ��ʼ����
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
