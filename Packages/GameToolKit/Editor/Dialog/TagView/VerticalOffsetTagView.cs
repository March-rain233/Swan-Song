using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Layout/VerticalOffset", false)]
    public class VerticalOffsetTagView : LengthTagView
    {
        public override string Tag => "voffset";

        protected override string _fieldName => "Vertical Offset";

        protected override List<string> choices => new List<string>() { "init", "em" };
    }
}
