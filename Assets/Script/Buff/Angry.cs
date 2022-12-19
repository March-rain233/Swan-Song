using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Angry : RoundBuff
{
    public int AttackAdd = 100;
    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.HurtCalculating += Unit_HurtCalculating;
        Unit.UnitData.AttackWrapper.AddValue += AttackAdd;
    }

    private void Unit_HurtCalculating(HurtCalculateEvent obj)
    {
        obj.Rate *= 0.7f;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.HurtCalculating -= Unit_HurtCalculating;
        Unit.UnitData.AttackWrapper.AddValue -= AttackAdd;
    }

    public override BuffData GetBuffData()
    {
        var data = base.GetBuffData();
        data.Name = "怒火";
        data.Description = $"受到的伤害减少<color=blue>30%</color>，力量值增加<color=red>{AttackAdd}</color>点";
        return data;
    }
}
