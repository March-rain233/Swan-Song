using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Play/Comma", true)]
    public class CommaTagView : LengthTagView
    {
        public override string Tag => "comma";

        protected override List<string> choices => new List<string>() { "init" };

        protected override string _fieldName => "Duration (second)";
    }
}
