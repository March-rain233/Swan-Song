using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;

public class DepolyGuider : GuiderBase
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
        if (obj.Index == nameof(DepolyPanel))
        {
            Process();
        }
    }

    void Process()
    {
        index += 1;
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .GetOrOpenPanel(nameof(DepolyPanel)) as DepolyPanel;

        switch (index)
        {
            case 0:
                GuiderManager.BeginGuide();
                panel.enabled = false;
                GuiderManager.SetText("这里是单位部署界面\n" +
                    "你需要部署单位到战斗棋盘上")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 1:
                GuiderManager.HighlightController(panel.transform.FindAll("Bar").transform as RectTransform)
                    .SetText("此处将列出当前的队伍成员，你可以点击选中")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 2:
                GuiderManager.HighlightController(panel.transform as RectTransform)
                    .SetText("地图上拥有绿色标记的图格为可放置图格\n" +
                    "你需要点击图格来放置角色，当点击已有单位的图格时将取消放置")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 3:
                GuiderManager.HighlightController(panel.transform.FindAll("BtnFinish").transform as RectTransform)
                    .SetText("当放置一个及以上的单位后，可以点击该按钮进入战斗")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 4:
                GuiderManager.EndGuide();
                panel.enabled = true;
                GuiderManager.SetInvoked(this);
                ServiceFactory.Instance.GetService<EventManager>()
                    .UnregisterCallback<PanelOpenEvent>(CheckGuider);
                break;
        }
    }
}
