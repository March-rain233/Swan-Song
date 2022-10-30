using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Layout/Space", true)]
    public class SpaceTagView : LengthTagView
    {
        //todo:���size��ǩ����Դ�С����
        public override string Tag => "space";

        protected override string _fieldName => "SpaceLength";
    }
}
