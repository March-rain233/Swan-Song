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
    [Tag("Layout/Line-Indent", false)]
    public class LineIndentTagView : LengthTagView
    {
        public override string Tag => "line-indent";

        protected override string _fieldName => "LineIndent";
    }
}
