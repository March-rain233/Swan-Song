using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System.Text.RegularExpressions;
using GameToolKit.Editor;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Layout/Margin", false)]
    public class MarginTagView : LengthTagView
    {
        public override string Tag => "margin";

        protected override string _fieldName => "Margin";
    }
}
