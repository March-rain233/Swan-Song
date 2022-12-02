using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class ArcaneChain:RoundBuff
{
    public int Value;
    protected override void OnDisable()
    {
        Unit.UnitData.DefenceWrapper.AddValue -= Value;
    }

    protected override void OnEnable()
    {
        Unit.UnitData.DefenceWrapper.AddValue += Value;
    }

    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "奥术枷锁";
        res.Description = $"秘法构筑的枷锁缠绕于身\n" +
            $"防御减少{Value}点";
        return res;
    }
}
