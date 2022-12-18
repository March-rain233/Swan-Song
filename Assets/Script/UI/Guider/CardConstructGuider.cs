using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;

public class CardConstructGuider : GuiderBase
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
        if (obj.Index == nameof(CardConstructPanel))
        {
            Process();
        }
    }

    void Process()
    {
        index += 1;
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .GetOrOpenPanel(nameof(CardConstructPanel)) as CardConstructPanel;

        switch (index)
        {
            case 0:
                GuiderManager.BeginGuide();
                panel.enabled = false;
                GuiderManager.SetText("这里是卡牌构筑界面，你需要在这里规划角色要携带的技能")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 1:
                GuiderManager.HighlightController(panel.transform.FindAll("Scroll View") as RectTransform)
                    .SetText("此处将列出该角色的牌库，已携带的卡牌将被描边标记\n" +
                    "当点击一张未被描边的卡牌时将把他编入卡组，再次点击将取消编入")
                    .SetTextAlign(GuiderManager.AlignType.Bottom)
                    .SetClickCallback(Process);
                break;
            case 2:
                GuiderManager.HighlightController(panel.transform.FindAll("Scroll View") as RectTransform)
                    .SetText("卡牌分为了通用和专属，对于同种的通用卡牌可以携带多张\n" +
                    "但同种的专属卡牌只允许携带一张")
                    .SetTextAlign(GuiderManager.AlignType.Bottom)
                    .SetClickCallback(Process);
                break;
            case 3:
                GuiderManager.HighlightController(panel.transform.FindAll("TxtCount") as RectTransform)
                    .SetText("此处将显示当前携带的卡牌数量\n" +
                    "还请注意，一个卡组的卡牌数量至少为5张最多为20张，请合理分配")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 4:
                GuiderManager.HighlightController(panel.transform.FindAll("Return") as RectTransform)
                    .SetText("当你配置完成后可以点击此处结束构筑")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 5:
                GuiderManager.EndGuide();
                panel.enabled = true;
                GuiderManager.SetInvoked(this);
                ServiceFactory.Instance.GetService<EventManager>()
                    .UnregisterCallback<PanelOpenEvent>(CheckGuider);
                break;
        }
    }
}
