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
        var type = typeList.OfType<ItemType>().ElementAt(UnityEngine.Random.Range(0, typeList.Length));
        Item item = new Item() { Type = type };
        switch (type)
        {
            case ItemType.Money:
                item.Value = UnityEngine.Random.Range(30, 150);
                break;
            case ItemType.Card:
                var cards = new List<(Card, UnitData)>();
                for(int i = 0; i < GameManager.Instance.GameData.Members.Count; i++)
                {
                    var member = GameManager.Instance.GameData.Members[i];
                    var card = CardPoolManager.Instance.DrawCard(
                        CardPoolManager.NormalPoolIndex,
                        member.UnitModel.PrivilegeDeckIndex,
                        member.UnitModel.CoreDeckIndex
                        );
                    cards.Add((card, member));
                }
                for(int i = 0; i < 1; ++i)
                {
                    var member = GameManager.Instance.GameData.Members[UnityEngine.Random.Range(0, GameManager.Instance.GameData.Members.Count)];
                    var card = CardPoolManager.Instance.DrawCard(
                        member.UnitModel.PrivilegeDeckIndex,
                        member.UnitModel.CoreDeckIndex
                        );
                    cards.Add((card, member));
                }
                item.Value = cards;
                break;
        }
        var panel = ServiceFactory.Instance.GetService<PanelManager>()
            .OpenPanel(nameof(BootyPanel)) as BootyPanel;
        panel.ShowItems(new List<Item>() { item });
        panel.Quitting += Finish;
    }
}

