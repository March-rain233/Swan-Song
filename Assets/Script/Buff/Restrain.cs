using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Restrain : RoundBuff
{
    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.CanMove = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.CanMove = true;
    }
}
