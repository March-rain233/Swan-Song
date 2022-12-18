using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class LuckCoin : Artifact
{
    protected override void OnDisable()
    {
        GameManager.Instance.GameStateChanged -= Instance_GameStateChanged;
    }

    private void Instance_GameStateChanged(GameState obj)
    {
        var sta = obj as BattleState;
        if (sta != null)
        {
            sta.BattleIniting += Sta_BattleIniting;
        }
    }

    private void Sta_BattleIniting()
    {
        var sta = GameManager.Instance.GetState<BattleState>();
        sta.BootyIniting += (list) =>
        {
            if(sta.RoundNumber <= 10)
            {
                var item = list.Find(p => p.Type == BattleState.ItemType.Money);
                item.Value = Mathf.CeilToInt((float)item.Value * 1.2f);

                BattleState.Item cardItem = new BattleState.Item()
                {
                    Type = BattleState.ItemType.Card,
                    Value = new List<(Card, UnitData)>(),
                };
                foreach(var m in GameManager.Instance.GameData.Members)
                {
                    var card = CardPoolManager.Instance.DrawCard(CardPoolManager.NormalPoolIndex, Card.CardRarity.Normal);
                    (item.Value as List<(Card, UnitData)>).Add((card, m));
                }
                list.Add(cardItem);
            }
            else
            {
                var item = list.Find(p => p.Type == BattleState.ItemType.Money);
                item.Value = Mathf.CeilToInt((float)item.Value *0.8f);
            }
        };
    }

    protected override void OnEnable()
    {
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
    }
}
