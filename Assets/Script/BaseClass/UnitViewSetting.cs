using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class UnitViewSetting : ScriptableSingleton<UnitViewSetting>
{
    public Dictionary<int, GameObject> UnitViews = new();
}
