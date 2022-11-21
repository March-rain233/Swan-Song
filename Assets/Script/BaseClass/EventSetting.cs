using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using GameToolKit.Dialog;

public class EventSetting : ScriptableSingleton<EventSetting>
{
    public Dictionary<PlaceType, DialogTree> EventDialogDic = new Dictionary<PlaceType, DialogTree>();
}
