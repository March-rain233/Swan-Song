using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;
using TMPro;

public class MemberPanel : PanelBase
{
    public RectTransform UnitListRoot;
    public UnitDataDetailView UnitDetailView;
    public ToggleGroup ToggleGroup;
    public Button BtnExit;

    public GameObject UnitSelectModel;

    public Color UnitSelectColor;
    public Color UnitUnselectColor;

    protected override void OnInit()
    {
        base.OnInit();
        foreach(var member in GameManager.Instance.GameData.Members)
        {
            var obj = Instantiate(UnitSelectModel, UnitListRoot);
            var face = obj.transform.Find("Face").GetComponent<Image>();
            var name = obj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var hpBar = obj.transform.Find("HpBar").GetComponent<HpBar>();
            var toggle = obj.GetComponent<Toggle>();
            var img = obj.GetComponent<Image>();
            face.sprite = member.Face;
            name.text = member.Name;
            hpBar.InitHpBar(member.Blood, member.BloodMax);
            img.color = UnitUnselectColor;
            toggle.isOn = false;
            toggle.group = ToggleGroup;
            toggle.onValueChanged.AddListener((v) =>
            {
                if (v)
                {
                    img.color = UnitSelectColor;
                    UnitDetailView.UnitData = member;
                    UnitDetailView.Refresh();
                }
                else
                {
                    img.color= UnitUnselectColor;
                }
            });
        }
        ToggleGroup.transform.GetComponentInChildren<Toggle>()
            .isOn = true;
        BtnExit.onClick.AddListener(() =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(this);
        });
        UnitDetailView.OnConstructSkills += (data) =>
        {
            var res = new List<(string, IEnumerable<(Card, UnitData)>)>();
            System.Func<Card.CardType, string, (string, IEnumerable<(Card, UnitData)>)> func = (type, name) =>
            {
                return (name, from card in data.Deck
                              where card.Type == type
                              select (card, data));
            };
            res.Add(func(Card.CardType.Attack, "¹¥»÷"));
            res.Add(func(Card.CardType.Heal, "ÖÎÓú"));
            res.Add(func(Card.CardType.Defence, "·ÀÓù"));
            res.Add(func(Card.CardType.Other, "ÆäËû"));
            return res;
        };
    }
}
