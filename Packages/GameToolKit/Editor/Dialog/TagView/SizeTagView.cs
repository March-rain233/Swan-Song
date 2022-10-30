using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Text/Size", false)]
    public class SizeTagView : LengthTagView
    {
        //todo:解决size标签的相对大小功能
        public override string Tag => "size";

        protected override string _fieldName => "FontSize";
    }
}
