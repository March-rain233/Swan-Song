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
    [Tag("Layout/Indent", false)]
    public class IndentTagView : LengthTagView
    {
        public override string Tag => "indent";

        protected override string _fieldName => "Indent";
    }
}
