using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Text/Size", false)]
    public class SizeTagView : LengthTagView
    {
        //todo:���size��ǩ����Դ�С����
        public override string Tag => "size";

        protected override string _fieldName => "FontSize";
    }
}
