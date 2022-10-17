using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;

public class UnitSetting : ScriptableSingleton<UnitSetting>
{
    public Dictionary<string, UnitModel> UnitDic = new();
    public List<UnitModel> PlayerList = new();
}
