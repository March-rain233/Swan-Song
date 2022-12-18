using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Dialog;
using GameToolKit;

internal class PlatFormNode : ProcessNode
{
    protected override void OnPlay()
    {
        int count = 4;
        int priCount = UnityEngine.Random.Range(1, count + 1);
        List<BattleState.Item> items = new List<BattleState.Item>();
        for(int j = 0; j < count; ++j, --priCount)
        {
            var cards = new List<(Card, UnitData)>();
            for (int i = 0; i < GameManager.Instance.GameData.Members.Count; i++)
            {
                var member = GameManager.Instance.GameData.Members[i];
                if (priCount > 0)
                {
                    var card = CardPoolManager.Instance.DrawCard(
                        ((member.UnitModel.PrivilegeDeckIndex, Card.CardRarity.Privilege, 1)));
                    cards.Add((card, member));
                }
                else
                {
                    var card = CardPoolManager.Instance.DrawCard(
                        ((CardPoolManager.NormalPoolIndex, Card.CardRarity.Normal, 1)));
                    cards.Add((card, member));
                }
            }
            items.Add(new BattleState.Item() { Type = BattleState.ItemType.Card, Value = cards });
        }

        var pm = ServiceFactory.Instance.GetService<PanelManager>();
        var panel = pm.OpenPanel(nameof(BootyPanel)) as BootyPanel;
        panel.ShowItems(items);
        panel.Quitting += Finish;
    }
}

