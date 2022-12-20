using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameToolKit;

public class UnitDataView:MonoBehaviour
{
    #region UI控件
    public Image Face;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public HpBar HpBar;
    public LayoutGroup BuffListView;

    public GameObject BuffViewModel;
    #endregion

    public void RefreshBuffList(IEnumerable<Buff.BuffData> buffDatas)
    {
        var views = new List<GameObject>();
        for(int i = 0; i < BuffListView.transform.childCount; i++)
        {
            views.Add(BuffListView.transform.GetChild(i).gameObject);
        }
        UIUtility.BindingData(views, buffDatas, () =>
        {
            return Instantiate(BuffViewModel, BuffListView.transform);
        },
        (obj) =>
        {
            Destroy(obj);
        },
        (data, obj, _) =>
        {
            var view = obj.GetComponent<BuffView>();
            view.Binding(data);
        });
    }

    public void RefreshData(UnitData data)
    {
        Face.sprite = data.Face;
        Name.text = data.Name;
        Description.text = string.IsNullOrEmpty(data.Description) ? "信息不明" : data?.Description;
        HpBar.MaxHp = data.BloodMax;
        HpBar.Hp = data.Blood;
    }

    public void RefreshDataWithoutAnim(UnitData data)
    {
        Face.sprite = data.Face;
        Name.text = data.Name;
        Description.text = string.IsNullOrEmpty(data.Description) ? "信息不明" : data?.Description;
        HpBar.InitHpBar(data.Blood, data.BloodMax);
    }
}
