using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Layout/Space", true)]
    public class SpaceTagView : LengthTagView
    {
        //todo:解决size标签的相对大小功能
        public override string Tag => "space";

        protected override string _fieldName => "SpaceLength";
    }
}
