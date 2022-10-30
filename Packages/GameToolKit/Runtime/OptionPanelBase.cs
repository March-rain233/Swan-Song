using GameToolKit.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit
{
    public abstract class OptionPanelBase : PanelBase, IOptionalView
    {
        public abstract void ShowOptions(List<OptionArgument> options, Action<int> onSelected);
    }
}
