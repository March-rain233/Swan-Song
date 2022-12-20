using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;

public class BattleGuider : GuiderBase
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
        if (obj.Index == nameof(BattlePanel))
        {
            Process();
        }
    }

    void Process()
    {
        index += 1;
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .GetOrOpenPanel(nameof(BattlePanel)) as BattlePanel;

        switch (index)
        {
            case 0:
                GuiderManager.BeginGuide();
                panel.enabled = false;
                GuiderManager.SetText("这里是战斗界面")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 1:
                GuiderManager.HighlightController(panel.transform.FindAll("Top").transform as RectTransform)
                    .SetText("此处将列出该回合的单位行动次序，先攻越高的单位越早行动")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 2:
                GuiderManager.HighlightController(panel.transform.FindAll("Round") as RectTransform)
                    .SetText("这里将显示当前回合数和灭亡之歌的限制回合\n" +
                    "当回合数大于等于限制回合时\n" +
                    "1.在场所有单位会每回合扣除等于20%最大血量的血量\n" +
                    "2.受到的治疗效果减少为原来的20%")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 3:
                GuiderManager.HighlightController(panel.transform.FindAll("Players").transform as RectTransform)
                    .SetText("这里将列出参战的对员\n" +
                    "点击某一角色时将把当前界面显示角色替换为选中角色")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 4:
                GuiderManager.HighlightController(panel.transform.FindAll("Ap").transform as RectTransform)
                    .SetText("这里将显示当前显示角色的行动点数量\n" +
                    "在每一回合开始时角色将恢复2点行动点（不超过上限）")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 5:
                GuiderManager.HighlightController(panel.transform.FindAll("HandsView").transform as RectTransform)
                    .SetText("这里将显示当前显示角色的手牌\n" +
                    "在每一回合开始时角色将从牌库中抽取两张卡牌")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 6:
                GuiderManager.HighlightController(panel.transform.FindAll("HandsView").transform as RectTransform)
                    .SetText("卡牌的左上角会显示释放所需要的行动点数量\n" +
                    "当当前行动点足够释放卡牌时，卡牌将被描边为蓝色")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 7:
                GuiderManager.HighlightController(panel.transform.FindAll("HandsView").transform as RectTransform)
                    .SetText("通过拖动可释放的卡牌，地图上将用黄色标记可释放图块\n" +
                    "将箭头拖向被标记的图块，地图将用红色标记卡牌的影响范围\n" +
                    "此时释放卡牌将发动技能效果，释放后的卡牌将进入弃牌堆")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 8:
                GuiderManager.HighlightController(panel.transform.FindAll("BtnDeck").transform as RectTransform)
                    .SetText("点击该按钮将显示牌库")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 9:
                GuiderManager.HighlightController(panel.transform.FindAll("BtnDiscard").transform as RectTransform)
                    .SetText("点击该按钮将显示弃牌堆\n" +
                    "当牌堆被抽空时，弃牌堆将被自动重新洗入牌库")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 10:
                GuiderManager.HighlightController(panel.transform.FindAll("TogMove").transform as RectTransform)
                    .SetText("点击该按钮将进入移动模式，地图上将用蓝色标记所有可移动到的图格\n" +
                    "点击被标记的图格可以移动至该图格，每一次移动需要消耗一点行动点\n" +
                    "每个单位有自己的最大移动范围，单位的移动会被敌对单位阻挡但不会被队友阻挡\n" +
                    "所以可以尝试通过围堵的方式限制住敌方单位的移动")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 11:
                GuiderManager.HighlightController(panel.transform.FindAll("BtnFinish").transform as RectTransform)
                    .SetText("点击该按钮将结束当前单位的行动")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 12:
                GuiderManager.HighlightController(panel.transform as RectTransform)
                    .SetText("点击地图上的单位将在右侧显示单位的详细数据")
                    .SetTextAlign(GuiderManager.AlignType.Middle)
                    .SetClickCallback(Process);
                break;
            case 13:
                GuiderManager.EndGuide();
                panel.enabled = true;
                GuiderManager.SetInvoked(this);
                ServiceFactory.Instance.GetService<EventManager>()
                    .UnregisterCallback<PanelOpenEvent>(CheckGuider);
                break;
        }
    }
}
