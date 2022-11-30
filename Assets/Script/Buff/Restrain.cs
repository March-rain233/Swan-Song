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

    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "束缚";
        res.Description = $"当前回合无法移动";
        return res;
    }
}
