using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;

public class UnitSetting : ScriptableSingleton<UnitSetting>
{
    public List<UnitModel> PlayerList = new();
}
