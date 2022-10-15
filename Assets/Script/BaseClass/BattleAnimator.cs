using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using GameToolKit;
using DG.Tweening;

public class BattleAnimator
{
    Dictionary<Unit, Animator> _unitViews = new();
    BattleState _battleState;
    Sequence _sequence;
    Transform _unitRoot;
    
    public BattleAnimator(BattleState battleState)
    {
        _battleState = battleState;
        _unitRoot = new GameObject("Unit Root").transform;
    }
    internal void Init()
    {
        _sequence = DOTween.Sequence();
        var mapRenderer = ServiceFactory.Instance.GetService<MapRenderer>();
        foreach(var unit in _battleState.UnitList)
        {
            var view = Object.Instantiate(UnitDataManager.Instance.UnitViews[unit.UnitData.ViewType], _unitRoot);
            _unitViews.Add(unit, view.GetComponent<Animator>());
            view.transform.position = mapRenderer.Grid
                .CellToWorld(new Vector3Int(unit.Position.x, unit.Position.y));
        }
    }
}