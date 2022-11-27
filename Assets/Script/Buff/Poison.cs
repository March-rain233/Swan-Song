using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Poison : RoundBuff
{
    public float Damage;
    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.TurnBeginning -= Unit_TurnBeginning;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.TurnBeginning += Unit_TurnBeginning;
    }

    private void Unit_TurnBeginning()
    {
        (Unit as IHurtable).Hurt(Damage, HurtType.FromBuff, this);
    }

    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "中毒";
        res.Description = $"每回合损失{Damage}血量";
        return res;
    }
}
