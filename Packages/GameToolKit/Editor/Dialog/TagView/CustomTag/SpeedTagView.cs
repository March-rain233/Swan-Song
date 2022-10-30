using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor {
    [Tag("Play/Text Speed", false)]
    public class SpeedTagView : LengthTagView
    {
        public override string Tag => "speed";

        protected override List<string> choices => new List<string>() { "init"};

        protected override string _fieldName => "Text Speed (character/second)";
    }
}
