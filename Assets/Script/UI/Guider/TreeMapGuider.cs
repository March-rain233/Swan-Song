using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;

public class TreeMapGuider : GuiderBase
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
        if (obj.Index == nameof(TreeMapView))
        {
            Process();
        }
    }

    void Process()
    {
        index += 1;
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .GetOrOpenPanel(nameof(TreeMapView)) as TreeMapView;

        switch (index)
        {
            case 0:
                GuiderManager.BeginGuide();
                GuiderManager.SetText("这里是路线规划界面\n" +
                    "一位优秀的指挥官应该学会如何带领自己的队伍驶向终点")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 1:
                GuiderManager.HighlightController(panel.transform.FindAll("Scroll View").transform as RectTransform)
                    .SetText("地图会在这片区域展开，你可以通过拖动来预览地图")
                    .SetTextAlign(GuiderManager.AlignType.Top)
                    .SetClickCallback(Process);
                break;
            case 2:
                GuiderManager.HighlightController(panel.transform.FindAll("Scroll View").transform as RectTransform)
                    .SetText("我们的目标是抵达道路尽头的头目节点并击溃头目进入下一章节\n" +
                    "直至杀死那位于皇宫中的这场灾厄的罪魁祸首")
                    .SetTextAlign(GuiderManager.AlignType.Top)
                    .SetClickCallback(Process);
                break;
            case 3:
                GuiderManager.HighlightController(panel.transform.FindAll("Scroll View").transform as RectTransform)
                    .SetText("通过点击高亮的节点，我们可以选择下一步该前进的路线")
                    .SetTextAlign(GuiderManager.AlignType.Top)
                    .SetClickCallback(Process);
                break;
            case 4:
                GuiderManager.HighlightController(panel.transform.FindAll("Scroll View").transform as RectTransform)
                    .SetText("节点主要分为战斗和功能型节点\n" +
                    "在战斗节点中取得胜利将给予金币等物质奖励\n" +
                    "在功能节点则会有不同的特殊事件，还请指挥官稍后自行尝试")
                    .SetTextAlign(GuiderManager.AlignType.Top)
                    .SetClickCallback(Process);
                break;
            case 5:
                GuiderManager.HighlightController(panel.transform.FindAll("MenuBar").transform as RectTransform)
                    .SetText("上方为菜单条，左侧会显示当前所拥有的金币数量")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 6:
                GuiderManager.HighlightController(panel.transform.FindAll("MenuBar").transform as RectTransform)
                    .SetText("而右侧设有设置按钮和队伍按钮\n" +
                    "分别可以打开游戏设置面板和队伍面板")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 7:
                GuiderManager.HighlightController(panel.transform.FindAll("MenuBar").transform as RectTransform)
                    .SetText("在最右边会显示当前章节，章节总共有三章\n" +
                    "Prelude：序曲\n" +
                    "Climax：高潮\n" +
                    "Coda：尾声")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 8:
                GuiderManager.HighlightController(null)
                    .SetText("现在，开始你的冒险吧")
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
