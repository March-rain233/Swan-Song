using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Boss : Unit
{
    public Boss(UnitData data, Vector2Int pos) : base(data, pos) { }

    public Boss(UnitModel model, Vector2Int pos) : base(model, pos) { }
}
