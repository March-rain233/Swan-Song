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
        UnitDetailView.BtnSkill.onClick.AddListener(() =>
        {
            var pm = ServiceFactory.Instance.GetService<PanelManager>()
                .OpenPanel(nameof(CardConstructPanel)) as CardConstructPanel;
            pm.Binding(UnitDetailView.UnitData);
        });
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
    }
}
