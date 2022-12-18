using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameToolKit.Dialog;
using GameToolKit;
using static BattleState;

internal class LotteryNode : ProcessNode
{
    protected override void OnPlay()
    {
        var typeList = Enum.GetValues(typeof(ItemType));
        bool isValiad = false;
        Item item = new Item();
        while (!isValiad)
        {
            var type = typeList.OfType<ItemType>().ElementAt(UnityEngine.Random.Range(0, typeList.Length));
            item.Type = type;
            switch (type)
            {
                case ItemType.Money:
                    item.Value = UnityEngine.Random.Range(20, 150);
                    isValiad = true;
                    break;
                case ItemType.Card:
                    var cards = new List<(Card, UnitData)>();
                    for (int i = 0; i < GameManager.Instance.GameData.Members.Count; i++)
                    {
                        var member = GameManager.Instance.GameData.Members[i];
                        var card = CardPoolManager.Instance.DrawCard(
                            (CardPoolManager.NormalPoolIndex, Card.CardRarity.Normal, 1),
                            (member.UnitModel.PrivilegeDeckIndex, Card.CardRarity.Privilege, 1)
                            );
                        cards.Add((card, member));
                    }
                    for (int i = 0; i < 1; ++i)
                    {
                        var member = GameManager.Instance.GameData.Members[UnityEngine.Random.Range(0, GameManager.Instance.GameData.Members.Count)];
                        var card = CardPoolManager.Instance.DrawCard(
                            (member.UnitModel.PrivilegeDeckIndex, Card.CardRarity.Privilege, 1)
                            );
                        cards.Add((card, member));
                    }
                    item.Value = cards;
                    isValiad = true;
                    break;
                case ItemType.Artifact:
                    var artifacts = ArtifactSetting.Instance.RemainArtifacts.ToList();
                    if(artifacts.Count <= 0)
                    {
                        break;
                    }
                    List<Artifact> list = new List<Artifact>();
                    for (int i = 0; i < 1 && artifacts.Count > 0; ++i)
                    {
                        var art = artifacts[UnityEngine.Random.Range(0, artifacts.Count)];
                        list.Add(art);
                    }
                    item.Value = list;
                    isValiad = true;
                    break;
            }
        }
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel(nameof(BootyPanel)) as BootyPanel;
        panel.ShowItems(new List<Item>() { item });
        panel.Quitting += Finish;
    }
}

