using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using GameToolKit;
using DG;

public class BattleAnimator
{
    Dictionary<Unit, Animator> _unitViews = new();
    BattleState _battleState;
    
    public BattleAnimator(BattleState battleState)
    {
        _battleState = battleState;
    }
    internal void Init()
    {

    }
}