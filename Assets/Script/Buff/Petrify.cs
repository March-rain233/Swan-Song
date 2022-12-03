using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class Petrify : RoundBuff
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.CanMove = false;
        Unit.HurtCalculating += Unit_HurtCalculating;
    }

    private void Unit_HurtCalculating(HurtCalculateEvent obj)
    {
        obj.Rate *= 0.7f;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.CanMove = true;
        Unit.HurtCalculating -= Unit_HurtCalculating;
    }
}

