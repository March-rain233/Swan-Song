using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using GameToolKit.Editor;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Layout/MonoSpacing", false)]
    public class MonoSpacingTagView : LengthTagView
    {
        public override string Tag => "mspace";

        protected override string _fieldName => "MonoSpacing";
        protected override List<string> choices => new List<string>() { "em", "init" };
    }
}
