using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;


internal class DaemonBride : RoundBuff
{
    public float DefennceRate = 0.9f;
    public int AttackRate = 100;
    public int ApAdd = 1;
    protected override void OnDisable()
    {
        Unit.HurtCalculating -= Unit_HurtCalculating;
        Unit.Preparing -= Unit_Preparing;
        ServiceFactory.Instance.GetService<EventManager>()
            .UnregisterCallback<HurtCalculateEvent>(AddAttack);
    }

    protected override void OnEnable()
    {
        Unit.HurtCalculating += Unit_HurtCalculating;
        Unit.Preparing += Unit_Preparing;
        ServiceFactory.Instance.GetService<EventManager>()
            .RegisterCallback<HurtCalculateEvent>(AddAttack);
    }

    private void AddAttack(HurtCalculateEvent calculateEvent)
    {
        if(calculateEvent.Source == Unit)
        {
            calculateEvent.Rate *= 1 + AttackRate;
        }
    }

    private void Unit_Preparing()
    {
        Unit.UnitData.ActionPoint += ApAdd;
    }

    private void Unit_HurtCalculating(HurtCalculateEvent obj)
    {
        obj.Rate *= DefennceRate;
    }

    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "恶魔契约";
        res.Description = $"陌生的力量充满了身体……\n" +
            $"受到伤害减少{DefennceRate * 100}%，造成的伤害增加{AttackRate * 100}%，" +
            $"每回合额外回复{ApAdd}点体力，{Time}回合后死亡";
        return res;
    }
}
