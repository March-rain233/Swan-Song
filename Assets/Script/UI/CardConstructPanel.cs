using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameToolKit;
internal class CardConstructPanel : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.Normal;
    public Button BtnBack;
    public Transform Root;
    public GameObject GroupModel;
    public GameObject NameModel;
    public TextMeshProUGUI TxtCount;
    public event Action Quitting;
    public UnitData UnitData;
    bool _isValiad => UnitData.BagIndex.Count >= 5 && UnitData.BagIndex.Count <= 20;
    protected override void OnInit()
    {
        base.OnInit();
        BtnBack.onClick.AddListener(() =>
        {
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(this);
            Quitting?.Invoke();
        });
    }

    public void Binding(UnitData data)
    {
        UnitData = data;
        var normalList = UnitData.Deck.Where(c => c.Rarity == Card.CardRarity.Normal);
        var priList = UnitData.Deck.Where(c => c.Rarity == Card.CardRarity.Privilege);

        {
            var nameView = Instantiate(NameModel, Root);
            nameView.SetActive(true);
            nameView.GetComponent<TextMeshProUGUI>().text = "通用";

            var groupView = Instantiate(GroupModel, Root);
            groupView.SetActive(true);
            var list = groupView.GetComponent<CardListView>();
            list.InittingCardView += (view) =>
            {
                var index = UnitData.Deck.IndexOf(view.Card);
                var outline = view.GetComponent<Outline>();
                view.Clicked += () =>
                {
                    if (UnitData.Bag.Contains(view.Card))
                    {
                        UnitData.BagIndex.Remove(index);
                    }
                    else
                    {
                        UnitData.BagIndex.Add(index);
                    }
                    outline.enabled = UnitData.Bag.Contains(view.Card);
                    RefreshTxtCount();
                    BtnBack.enabled = _isValiad;
                };
                outline.enabled = UnitData.Bag.Contains(view.Card);
            };
            list.ShowCardList(normalList.Select(c=>(c, data)));
        }
        {
            var nameView = Instantiate(NameModel, Root);
            nameView.SetActive(true);
            nameView.GetComponent<TextMeshProUGUI>().text = "专属";

            var groupView = Instantiate(GroupModel, Root);
            groupView.SetActive(true);
            var list = groupView.GetComponent<CardListView>();
            list.InittingCardView += (view) =>
            {
                var index = UnitData.Deck.IndexOf(view.Card);
                var outline = view.GetComponent<Outline>();
                view.Clicked += () =>
                {
                    if (UnitData.Bag.Contains(view.Card))
                    {
                        UnitData.BagIndex.Remove(index);
                    }
                    else if (UnitData.Bag.FirstOrDefault(c => c.GetType() == view.Card.GetType()) == null)
                    {
                        UnitData.BagIndex.Add(index);
                    }
                    outline.enabled = UnitData.Bag.Contains(view.Card);
                    RefreshTxtCount();
                    BtnBack.enabled = _isValiad;
                };
                outline.enabled = UnitData.Bag.Contains(view.Card);
            };
            list.ShowCardList(priList.Select(c => (c, data)));
        }

        BtnBack.enabled = _isValiad;
    }
    void RefreshTxtCount()
    {
        TxtCount.text = $"<color={(_isValiad ? "green" : "red")}>{UnitData.BagIndex.Count}/20</color>";
    }
}
