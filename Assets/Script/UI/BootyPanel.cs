using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using static BattleState;

public class BootyPanel : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.HideOther;

    public Button BtnNext;
    public LayoutGroup ItemList;
    public GameObject ItemModel;

    public event System.Action Quitting;

    List<GameObject> _items;

    protected override void OnInit()
    {
        base.OnInit();
        BtnNext.onClick.AddListener(Next);
    }

    public void ShowItems(IEnumerable<Item> items)
    {
        _items = new();
        foreach (var item in items)
        {
            var obj = Instantiate(ItemModel, ItemList.transform);
            _items.Add(obj);
            var body = obj.transform.GetComponentInChildren<TextMeshProUGUI>();
            var button = obj.GetComponent<Button>();
            switch (item.Type)
            {
                case ItemType.Money:
                    body.text = $"获得{item.Value}金币";
                    button.onClick.AddListener(() =>
                    {
                        GameManager.Instance.GameData.Gold += (int)item.Value;
                        DeleteItem(obj);
                    });
                    break;
                case ItemType.Card:
                    body.text = "获取技能";
                    button.onClick.AddListener(() =>
                    {
                        var pm = ServiceFactory.Instance.GetService<PanelManager>();
                        var panel =  pm.OpenPanel(nameof(CardSelectPanel)) as CardSelectPanel;
                        panel.SetCards(item.Value as List<(Card, UnitData)>);
                        panel.CardSelected += (card, data) =>
                        {
                            data.Deck.Add(card);
                            pm.ClosePanel(panel);
                            DeleteItem(obj);
                        };
                    });
                    break;
                case ItemType.Artifact:
                    body.text = "获取遗物";
                    button.onClick.AddListener(() =>
                    {
                        var pm = ServiceFactory.Instance.GetService<PanelManager>();
                        var panel = pm.OpenPanel(nameof(ArtifactSelectPanel)) as ArtifactSelectPanel;
                        panel.SetOption(item.Value as IEnumerable<Artifact>);
                        panel.Selected += (a) =>
                        {
                            GameManager.Instance.AddArtifact(a);
                            pm.ClosePanel(panel);
                            DeleteItem(obj);
                        };
                    });
                    break;
            }
        }
    }

    void DeleteItem(GameObject gameObject)
    {
        Destroy(gameObject);
        _items.Remove(gameObject);
        if(_items.Count == 0)
        {
            Next();
        }
    }

    void Next()
    {
        ServiceFactory.Instance.GetService<PanelManager>()
            .ClosePanel(this);
        Quitting?.Invoke();
    }
}
