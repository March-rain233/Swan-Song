using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Events;

public class GuiderManager : IService
{
    public enum AlignType
    {
        Top,
        Middle,
        Bottom,
    }

    Dictionary<int, GuiderBase> _guiderList = new()
    {
        { 1, new PlayerSelectGuider() },
        { 2, new TreeMapGuider() },
        { 3, new DepolyGuider() },
        { 4, new BattleGuider() },
        { 5, new CardConstructGuider() },
    };
    public string Path => Application.persistentDataPath + "/guider.data";

    public GuiderPanel Panel { get; private set; }

    void IService.Init()
    {
        var list = GetInvokedList();
        foreach (var guider in _guiderList.Where(p=>!list.Contains(p.Key))
            .Select(p=>p.Value))
        {
            guider.Init();
        }
    }

    public void BeginGuide()
    {
        Panel = ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel(nameof(GuiderPanel)) as GuiderPanel;
    }

    public void EndGuide()
    {
        ServiceFactory.Instance.GetService<PanelManager>()
            .ClosePanel(Panel);
        Panel = null;
    }

    public GuiderManager HighlightController(RectTransform rectTransform)
    {
        Panel.HollowOutMask.m_HollowOutArea = rectTransform;
        Panel.HollowOutMask.IsPenetrate = false;
        return this;
    }

    public GuiderManager SetClickCallback(Action callback)
    {
        Panel.TxtBody.text += "\n<size=12><align=right>—点击继续—</align></size>";
        InputSystem.onEvent
            .ForDevice<Pointer>()
            .Where(e => e.HasButtonPress())
            .CallOnce(ctrl=>callback());
        return this;
    }

    public GuiderManager HighlightButton(Button button, Action callback)
    {
        HighlightController(button.transform as RectTransform);

        UnityAction once = null;
        once = () =>
        {
            callback();
            button.onClick.RemoveListener(once);
        };
        button.onClick.AddListener(once);

        Panel.HollowOutMask.IsPenetrate = true;
        return this;
    }

    public GuiderManager SetText(string text)
    {
        Panel.TxtBody.text = text;
        return this;
    }

    public GuiderManager SetTextAlign(AlignType alignType)
    {
        switch (alignType)
        {
            case AlignType.Top:
                Panel.TextRoot.pivot = new Vector2(0.5f, 1f);
                Panel.TextRoot.anchorMax = Panel.TextRoot.anchorMin = new Vector2(0.5f, 1f);
                Panel.TextRoot.anchoredPosition = new Vector2(0, -10f);
                break;
            case AlignType.Middle:
                Panel.TextRoot.pivot = new Vector2(0.5f, 0.5f);
                Panel.TextRoot.anchorMax = Panel.TextRoot.anchorMin = new Vector2(0.5f, 0.5f);
                Panel.TextRoot.anchoredPosition = new Vector2(0, 0);
                break;
            case AlignType.Bottom:
                Panel.TextRoot.pivot = new Vector2(0.5f, 0f);
                Panel.TextRoot.anchorMax = Panel.TextRoot.anchorMin = new Vector2(0.5f, 0f);
                Panel.TextRoot.anchoredPosition = new Vector2(0, 10f);
                break;
        }
        return this;
    }

    public GuiderManager SetTextSize(float size)
    {
        Panel.TxtBody.fontSize = size;
        return this;
    }

    public void SetInvoked(GuiderBase guiderBase)
    {
        var list = GetInvokedList();
        list = list.Append(_guiderList.First(p => p.Value == guiderBase).Key);
        var text = list.Aggregate("", (str, v) => str + $"{v} ");
        File.WriteAllText(Path, text);
    }

    /// <summary>
    /// 获取已被激活的列表
    /// </summary>
    /// <returns></returns>
    IEnumerable<int> GetInvokedList()
    {
        try
        {
            return File.ReadAllText(Path)
                .Split(' ')
                .Where(s=>!string.IsNullOrEmpty(s))
                .Select(t=>int.Parse(t));
        }
        catch(Exception ex)
        {
            return Enumerable.Empty<int>();
        }
    }

    /// <summary>
    /// 重置指导
    /// </summary>
    public void Reset()
    {
        var list = GetInvokedList();
        foreach(var guider in list.Select(i => _guiderList[i]))
        {
            guider.Init();
        }
        File.Delete(Path);
    }
}
