using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class Bloodlust : RoundBuff
{
    public float BloodRate = 0.9f;
    public int AttackAdd = 100;
    protected override void OnDisable()
    {
        base.OnDisable();
        Unit.UnitData.BloodMaxWrapper.Rate /= BloodRate;
        Unit.UnitData.AttackWrapper.AddValue -= AttackAdd;
        Unit.UnitData.RefreshBlood();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Unit.UnitData.BloodMaxWrapper.Rate *= BloodRate;
        Unit.UnitData.AttackWrapper.AddValue += AttackAdd;
        Unit.UnitData.RefreshBlood();
    }

    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "血液狂热";
        res.Description = $"从伤口中迸射的血液凝聚成刀刃……\n" +
            $"降低<color=red>{(1 -BloodRate) * 100}%</color>最大生命值，力量值增加<color=red>{AttackAdd}</color>点";
        return res;
    }
}

