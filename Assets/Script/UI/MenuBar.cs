using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameToolKit;

public class MenuBar : MonoBehaviour
{
    public TextMeshProUGUI TxtCoin;
    public TextMeshProUGUI TxtChapter;
    public Button BtnSetting;
    public Button BtnMembers;

    private void Awake()
    {
        var pm = ServiceFactory.Instance.GetService<PanelManager>();
        GameManager.Instance.GameData.GoldChanged += GameData_GoldChanged;
        GameData_GoldChanged();
        BtnMembers.onClick.AddListener(() =>
        {
            pm.OpenPanel(nameof(MemberPanel));
        });
        BtnSetting.onClick.AddListener(() =>
        {
            pm.OpenPanel(nameof(SettingPanel));
        });
        TxtChapter.text = GameManager.Instance.GameData.Chapter switch
        {
            1 => "Prelude",
            2 => "Climax",
            3 => "Coda",
            _ => "Unknow"
        };
    }

    private void GameData_GoldChanged()
    {
        TxtCoin.text = GameManager.Instance.GameData.Gold.ToString();
    }

    private void OnDestroy()
    {
        GameManager.Instance.GameData.GoldChanged -= GameData_GoldChanged;
    }
}
