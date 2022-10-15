using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit;

public class UnitDataManager : ScriptableSingleton<UnitDataManager>
{
    public Dictionary<int, UnitModel> Models = new();
    public Dictionary<int, GameObject> UnitViews = new();
}
