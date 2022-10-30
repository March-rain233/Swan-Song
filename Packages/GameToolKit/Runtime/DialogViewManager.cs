using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameToolKit.Dialog
{
    public class DialogViewManager : IDialogViewManager
    {
        public IDialogBox GetDialogBox(Type type)
        {
            return ServiceFactory.Instance.GetService<PanelManager>()
                .GetOrOpenPanel(type.Name.Split('.').Last()) as IDialogBox;
        }

        public IOptionalView GetOptionalView(Type type)
        {
            return ServiceFactory.Instance.GetService<PanelManager>()
                .GetOrOpenPanel(type.Name.Split('.').Last()) as IOptionalView;
        }
    }
}
