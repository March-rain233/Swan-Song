using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using GameToolKit.Editor;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Layout/Cspace", false)]
    public class CspaceTagView : LengthTagView
    {
        public override string Tag => "cspace";

        protected override string _fieldName => "Cspace";

        protected override List<string> choices => new List<string>() { "init","em"  };
    }
}
