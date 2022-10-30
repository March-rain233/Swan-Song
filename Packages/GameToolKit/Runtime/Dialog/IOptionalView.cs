using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// ѡ���ӿ�
    /// </summary>
    public interface IOptionalView
    {
        public abstract void ShowOptions(List<OptionArgument> options, Action<int> onSelected);
    }
}
