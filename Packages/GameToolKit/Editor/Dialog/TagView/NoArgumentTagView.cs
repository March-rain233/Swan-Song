using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor
{
    /// <summary>
    /// �޲�����ǩ����
    /// </summary>
    public abstract class NoArgumentTagView : TagView
    {
        public override string ConvertToText()
        {
            return Tag;
        }

        public override bool SetValueFromText(string attr)
        {
            return attr == Tag;
        }
    }
}