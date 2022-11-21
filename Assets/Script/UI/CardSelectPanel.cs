using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using UnityEngine.UI;
using System;

public class CardSelectPanel : PanelBase
{
    public Button ReturnButton;
    public LayoutGroup Root;

    public event Action Returned;

    public void Init(IEnumerable<Card> cards)
    {
        var model = UISetting.Instance.PrefabsDic["CardView"];
    }
}
