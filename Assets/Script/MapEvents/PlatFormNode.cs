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
        var cards = new List<(Card, UnitData)>();
        for (int i = 0; i < GameManager.Instance.GameData.Members.Count; i++)
        {
            var member = GameManager.Instance.GameData.Members[i];
            var card = CardPoolManager.Instance.DrawCard(
                member.UnitModel.PrivilegeDeckIndex,
                member.UnitModel.CoreDeckIndex
                );
            cards.Add((card, member));
        }
        for (int i = 0; i < 1; ++i)
        {
            var member = GameManager.Instance.GameData.Members[UnityEngine.Random.Range(0, GameManager.Instance.GameData.Members.Count)];
            var card = CardPoolManager.Instance.DrawCard(
                member.UnitModel.CoreDeckIndex
                );
            cards.Add((card, member));
        }

        var pm = ServiceFactory.Instance.GetService<PanelManager>();
        var panel = pm.OpenPanel(nameof(CardSelectPanel)) as CardSelectPanel;
        panel.BtnBack.gameObject.SetActive(false);
        panel.SetCards(cards);
        panel.CardSelected += (card, user) =>
        {
            user.Deck.Add(card);
            pm.ClosePanel(panel);
            Finish();
        };
    }
}

