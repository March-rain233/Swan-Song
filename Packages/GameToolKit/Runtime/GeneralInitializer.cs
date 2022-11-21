using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit
{
    /// <summary>
    /// Ĭ�ϳ�ʼ����
    /// </summary>
    public class GeneralInitializer : ServiceInitializer
    {
        public override void Initialize()
        {
            var instance = ServiceFactory.Instance;
            instance.Register<EventManager, EventManager>();
            instance.Register<PanelManager, PanelManager>();
            instance.Register<Dialog.IDialogViewManager, Dialog.DialogViewManager>();
            instance.Register<Dialog.DialogManager, Dialog.DialogManager>();
        }
    }
}
