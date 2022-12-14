using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;

public class PlayerSelectGuider : GuiderBase
{
    int index = -1;
    public override void Init()
    {
        index = -1;
        ServiceFactory.Instance.GetService<EventManager>()
            .RegisterCallback<PanelOpenEvent>(CheckGuider);
    }

    private void CheckGuider(PanelOpenEvent obj)
    {
        if(obj.Index == nameof(PlayerSelectPanel))
        {
            Process();
        }
    }

    void Process()
    {
        index += 1;
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .GetOrOpenPanel(nameof(PlayerSelectPanel)) as PlayerSelectPanel;

        switch (index)
        {
            case 0:
                GuiderManager.BeginGuide();
                GuiderManager.SetText("呦，又是一个新的指挥官……\n" +
                    "我是引导精灵Navi，我将指导你如何进行一场成功的冒险")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 1:
                GuiderManager.HighlightController(panel.transform.FindAll("SelectList").transform as RectTransform)
                    .SetText("下方是可选成员列表。\n" +
                    "点击你所青睐的人物头像，他的信息将会显示于上方")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 2:
                GuiderManager.HighlightController(panel.transform.FindAll("DataViewGroup").transform as RectTransform)
                    .SetText("此处将显示选中人物的能力数值，数值的高低可以一定程度上反映一个角色的强度")
                    .SetTextAlign(GuiderManager.AlignType.Bottom)
                    .SetClickCallback(Process);
                break;
            case 3:
                GuiderManager.HighlightController(panel.transform.FindAll("DataViewGroup").transform as RectTransform)
                    .SetText("<size=14>体力：角色的最高血量\n" +
                    "腕力：角色奋力攻击的强度（攻击力）\n" +
                    "坚韧：角色对伤害的忍受能力（防御力）\n" +
                    "虔诚：角色对进行治疗行为时的恢复力度\n" +
                    "先攻：角色在战场上的敏锐程度，该项数值越高，角色越早行动\n" +
                    "行动：角色的最大行动点数（角色在进行攻击、移动等行为时需要消耗行动点）</size>")
                    .SetTextAlign(GuiderManager.AlignType.Bottom)
                    .SetClickCallback(Process);
                break;
            case 4:
                GuiderManager.HighlightController(panel.transform.FindAll("Level").transform as RectTransform)
                    .SetText("此处是角色的等级预览\n点击右侧的按钮可以预览角色在不同等级下的属性变化")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 5:
                GuiderManager.HighlightController(panel.transform.FindAll("BtnSkill").transform as RectTransform)
                    .SetText("<size=14>点击此处的按钮可以查看角色可以学会的技能，技能分为三个种类\n" +
                    "通用：所有角色都能学会的技能\n" +
                    "专属：只有该角色能学会的技能\n" +
                    "核心：该角色专属的一些强力技能</size>")
                    .SetTextAlign(GuiderManager.AlignType.Top)
                    .SetClickCallback(Process);
                break;
            case 6:
                GuiderManager.HighlightController(panel.transform.FindAll("BtnJoin").transform as RectTransform)
                    .SetText("点击加入按钮可以让当前选中的角色进入冒险队伍")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 7:
                GuiderManager.HighlightController(panel.transform.FindAll("BtnDelete").transform as RectTransform)
                    .SetText("点击取消按钮可以让当前选中的角色退出冒险队伍")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 8:
                GuiderManager.HighlightController(panel.transform.FindAll("JoinList").transform as RectTransform)
                    .SetText("此处将显示当前队伍的组成\n冒险需要1-3名成员才可以进行")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 9:
                GuiderManager.HighlightController(panel.transform.FindAll("BtnComplete").transform as RectTransform)
                    .SetText("点击出发按钮将以当前选择的队伍开始冒险")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 10:
                GuiderManager.HighlightController(null)
                    .SetText("关于队伍的组成，我知道的就是这些了\n现在，该开始你自己的冒险了")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(EndGuide);
                break;
        }
    }

    private void EndGuide()
    {
        GuiderManager.EndGuide();
        GuiderManager.SetInvoked(this);
        ServiceFactory.Instance.GetService<EventManager>()
            .UnregisterCallback<PanelOpenEvent>(CheckGuider);
    }
}
