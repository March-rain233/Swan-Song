using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class StoreState : GameState
{
    public const int NormalCost = 20;
    public const int PrivilegeCost = 60;
    public const int CoreCost = 120;
    public const int RelicCost = 120;

    protected internal override void OnEnter()
    {
        var coreCard = CardPoolManager.Instance.DrawCard(GameManager.Instance.GameData.Members.Select(u => u.UnitModel.CoreDeckIndex).ToArray());

    }

    protected internal override void OnExit()
    {

    }

    protected internal override void OnUpdata()
    {

    }
}
