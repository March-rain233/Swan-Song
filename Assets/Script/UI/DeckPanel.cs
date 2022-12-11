using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameToolKit;
internal class DeckPanel : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.Normal;
    public Button BtnBack;
    public Transform Root;
    public GameObject GroupModel;
    public GameObject NameModel;
    public event Action Quitting;

    protected override void OnInit()
    {
        base.OnInit();
        FindObjectOfType<ViewPoint>().enabled = false;
        BtnBack.onClick.AddListener(() =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(this);
            Quitting?.Invoke();
        });
    }
    protected override void OnDispose()
    {
        base.OnDispose();
        FindObjectOfType<ViewPoint>().enabled = true;
    }
    public void ShowCardList(IEnumerable<(string name, IEnumerable<(Card, UnitData)> list)> list)
    {
        foreach (var group in list)
        {
            var nameView = Instantiate(NameModel, Root);
            nameView.SetActive(true);
            nameView.GetComponent<TextMeshProUGUI>().text = group.name;

            var groupView = Instantiate(GroupModel, Root);
            groupView.SetActive(true);
            groupView.GetComponent<CardListView>()
                .ShowCardList(group.list);
        }
    }
}
