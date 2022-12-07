using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;
internal class SoulLink : Buff
{
    public Unit User;
    protected override void OnDisable()
    {
        ServiceFactory.Instance.GetService<EventManager>()
            .UnregisterCallback<HurtCalculateEvent>(Handler);
    }

    protected override void OnEnable()
    {
        ServiceFactory.Instance.GetService<EventManager>()
            .RegisterCallback<HurtCalculateEvent>(Handler);
    }

    void Handler(HurtCalculateEvent calculateEvent)
    {
        if(calculateEvent.Source == Unit)
        {
            (User as ICurable).Cure(20, this);
        }
    }

    public override BuffData GetBuffData()
    {
        var res = base.GetBuffData();
        res.Name = "灵魂连接";
        res.Description = $"此角色每造成一次伤害，就回复{User.UnitData.Name}<color=green>20</color>点生命";
        return res;
    }
}
