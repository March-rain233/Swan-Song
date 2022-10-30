using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Layout/Width", false)]
    public class WidthTagView : LengthTagView
    {
        public override string Tag => "width";

        protected override string _fieldName => "Width";

        protected override List<string> choices => new List<string>() { "init", "%"  };
    }
}
