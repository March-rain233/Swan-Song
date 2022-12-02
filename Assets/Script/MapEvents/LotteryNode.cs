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
                break;
        }
    }
}

