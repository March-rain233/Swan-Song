using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameToolKit;

public class UnitDataDetailView : MonoBehaviour
{
    public UnitData UnitData;

    public TextMeshProUGUI TxtBlood;
    public TextMeshProUGUI TxtAttack;
    public TextMeshProUGUI TxtDefence;
    public TextMeshProUGUI TxtHeal;
    public TextMeshProUGUI TxtSpeed;
    public TextMeshProUGUI TxtAP;
    public TextMeshProUGUI TxtLevel;
    public TextMeshProUGUI TxtDescription;
    public Button BtnLevelUp;
    public Button BtnLevelDown;
    public TextMeshProUGUI Name;
    public Image Face;
    public Button BtnSkill;

    private void Awake()
    {
        BtnLevelDown.onClick.AddListener(() =>
        {
            UnitData.SetLevel(UnitData.Level - 1);
            if (UnitData.Level == 1)
            {
                BtnLevelDown.interactable = false;
            }
            BtnLevelUp.interactable = true;
            Refresh();
        });
        BtnLevelUp.onClick.AddListener(() =>
        {
            UnitData.SetLevel(UnitData.Level + 1);
            if (UnitData.Level >= GameManager.MaxLevel)
            {
                BtnLevelUp.interactable = false;
            }
            BtnLevelDown.interactable = true;
            Refresh();
        });
    }

    public void Refresh()
    {
        Name.text = UnitData.Name;
        Face.sprite = UnitData.Face;
        TxtBlood.text = $"{UnitData.BloodMax}";
        TxtAttack.text = $"{UnitData.Attack}";
        TxtDefence.text = $"{UnitData.Defence}";
        TxtSpeed.text = $"{UnitData.Speed}";
        TxtHeal.text = $"{UnitData.Heal}";
        TxtAP.text = $"{UnitData.ActionPointMax}";
        TxtLevel.text = $"Level {(UnitData.Level >= GameManager.MaxLevel ? "MAX" : UnitData.Level)}";
        TxtDescription.text = string.IsNullOrEmpty(UnitData.Description) ? "信息不明" : UnitData?.Description;
    }
}
