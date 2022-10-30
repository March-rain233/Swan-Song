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
    [Tag("Layout/HorizontalPosition", false)]
    public class HorizontalPositionTagView : LengthTagView
    {
        public override string Tag => "pos";

        protected override string _fieldName => "HorizontalPosition";
    }
}
