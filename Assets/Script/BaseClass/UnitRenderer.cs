using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 单位渲染器
/// </summary>
public class UnitRenderer
{
    Transform _unitRoot;
    public IReadOnlyList<UnitView> UnitViews => _unitViews;
    List<UnitView> _unitViews = new();
    BattleState _battleState;
    public UnitRenderer(BattleState battleState)
    {
        _battleState = battleState;
        _unitRoot = new GameObject("Unit Root").transform;
    }

    /// <summary>
    /// 销毁服务
    /// </summary>
    public void Destroy()
    {
        Object.Destroy(_unitRoot.gameObject);
    }

    /// <summary>
    /// 创建单位视图
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public UnitView CreateUnitView(Unit unit)
    {
        var model = Object.Instantiate(UnitViewSetting.Instance.UnitViews[unit.UnitData.ViewType],
            _unitRoot);
        var view = model.GetComponent<UnitView>();
        view.Unit = unit;

        _unitViews.Add(view);

        view.transform.position = _battleState
            .MapRenderer.Grid.CellToWorld(unit.Position.ToVector3Int());
        _battleState.Animator.BindindUnitAnimation(unit, view);
        return view;
    }

    /// <summary>
    /// 选中单位
    /// </summary>
    /// <param name="units"></param>
    public void SelectUnit(IEnumerable<Unit> units = null)
    {
        if (units != null)
        {
            foreach (var unit in _unitViews)
            {
                if (units.Contains(unit.Unit))
                {
                    unit.Flag();
                }
                else
                {
                    unit.Unflag();
                }
            }
        }
        else
        {
            foreach (var view in _unitViews)
            {
                view.Unflag();
            }
        }
    }
}
