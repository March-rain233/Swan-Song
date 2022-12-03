using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class Endure : Buff
{
    protected override void OnDisable()
    {
        Unit.Hurt -= Unit_Hurt;
    }

    protected override void OnEnable()
    {
        Unit.Hurt += Unit_Hurt;
    }

    private void Unit_Hurt(float arg1, HurtType arg2, object arg3)
    {
        if(arg2.HasFlag(HurtType.FromUnit | HurtType.Melee))
        {
            (arg3 as IHurtable).Hurt(Unit.UnitData.Attack, HurtType.FromUnit | HurtType.AD | HurtType.Melee, Unit);
            Unit.RemoveBuff(this);
        }
    }

    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "忍耐";
        res.Description = "遭遇敌人的近身攻击时，对其造成<color=red>100%</color>力量值的伤害";
        return res;
    }
}
