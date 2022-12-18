using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;
using GameToolKit.Dialog;
internal class OpenCardPackNode : ProcessNode
{
    [Flags]
    public enum PackType
    {
        Universal = 1,
        Privilege = 1 << 1,
    }

    public PackType Type;

    [Port("Index", PortDirection.Input)]
    public int Index;

    public int Num = 3;
    protected override void OnPlay()
    {
        var member = GameManager.Instance.GameData.Members[Index];
        var model = member.UnitModel;
        List<(string, Card.CardRarity, int)> poolIndexs = new();
        List<Card> cards = new();
        if (Type.HasFlag(PackType.Universal))
        {
            poolIndexs.Add((CardPoolManager.NormalPoolIndex, Card.CardRarity.Normal, 1));
        }
        if (Type.HasFlag(PackType.Privilege))
        {
            poolIndexs.Add((member.UnitModel.PrivilegeDeckIndex, Card.CardRarity.Privilege, 1));
        }

        for(int i = 0; i < Num; i++)
        {
            cards.Add(CardPoolManager.Instance.DrawCard(poolIndexs.ToArray()));
        }

        var pm = ServiceFactory.Instance.GetService<PanelManager>();
        var panel = pm.OpenPanel(nameof(CardSelectPanel)) as CardSelectPanel;
        panel.BtnBack.gameObject.SetActive(false);
        panel.SetCards(cards.Select(c => (c, member)));
        panel.CardSelected += (card, user) =>
        {
            user.Deck.Add(card);
            pm.ClosePanel(panel);
            Finish();
        };
    }
}
