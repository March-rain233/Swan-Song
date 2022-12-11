using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolKit;
using UnityEngine.InputSystem;
using System.Linq;

public class DepolyPanel : PanelBase
{
    public override PanelShowType ShowType => PanelShowType.Normal;

    public Button BtnComplete;
    public ToggleGroup UnitSelectGroup;

    public GameObject ToggleModel;

    List<Vector2Int> _points;
    List<UnitData> _units;
    Dictionary<UnitData, Vector2Int> _result = new();
    Dictionary<UnitData, GameObject> _views = new();

    UnitSelectView CurrentSelect => UnitSelectGroup.GetFirstActiveToggle()
            .GetComponent<UnitSelectView>();
    protected override void OnInit()
    {
        base.OnInit();
        var gm = ServiceFactory.Instance.GetService<GameManager>();
        var state = gm.GetState() as BattleState;
        BtnComplete.interactable = false;
        BtnComplete.onClick.AddListener(() =>
        {
            var list = _result.Select(p => (p.Key, p.Value));
            state.DeployUnits(list);
        });
        state.DeployBeginning += DepolyPanel_DeployBeginning;
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        var state = GameManager.Instance.GetState<BattleState>();
        var mr = state.MapRenderer;
        foreach(var unit in _views)
        {
            Destroy(unit.Value);
        }
        state.DeployBeginning -= DepolyPanel_DeployBeginning;
        mr.RenderDepolyTile();
    }

    private void DepolyPanel_DeployBeginning(List<Vector2Int> arg1, List<UnitData> arg2)
    {
        _points = arg1;
        _units = arg2;
        var mr = GameManager.Instance.GetState<BattleState>().MapRenderer;
        mr.RenderDepolyTile(arg1);
        foreach (var p in arg2)
        {
            CreateSelect(p);
        }

        CurrentSelect.Toggle.onValueChanged.Invoke(true);
    }

    void CreateSelect(UnitData unit)
    {
        var model = Instantiate(ToggleModel, UnitSelectGroup.transform);
        var view = model.GetComponent<UnitSelectView>();
        view.Binding(unit);
        view.Toggle.group = UnitSelectGroup;
    }

    private void Update()
    {
        var mr = GameManager.Instance.GetState<BattleState>().MapRenderer;
        var mouse = TileUtility.GetMouseInCell();
        mouse.z = 0;
        var pos = new Vector2Int(mouse.x, mouse.y);
        //如果点击了可选择的图块，且该图块还未被单位占据，则将当前选中单位放置于该图块
        //如果该图块占据的单位就是当前单位，则取消单位参战状态
        if (Pointer.current.press.wasReleasedThisFrame && _points.Contains(pos))
        {
            if (_result.ContainsValue(pos))
            {
                if(_result.First(p=>p.Value == pos).Key == CurrentSelect.UnitData)
                {
                    CancelUnit();
                }
            }
            else
            {
                SetUnit(pos);
            }
        }
    }

    void SetUnit(Vector2Int pos)
    {
        var view = CurrentSelect;
        view.Join();
        _result[view.UnitData] = pos;

        GameObject unitView;
        if(!_views.TryGetValue(view.UnitData, out unitView))
        {
            unitView = Instantiate(UnitDataManager.Instance.UnitViews[view.UnitData.ViewType]);
            _views.Add(view.UnitData, unitView);
        }
        unitView.transform.position = GameManager.Instance.GetState<BattleState>().MapRenderer
            .Grid.CellToWorld(new Vector3Int(pos.x, pos.y));

        BtnComplete.interactable = true;
    }

    void CancelUnit()
    {
        var view = CurrentSelect;
        view.Unjoin();
        _result.Remove(view.UnitData);

        Destroy(_views[view.UnitData]);
        _views.Remove(view.UnitData);

        if(_result.Count < 1)
        {
            BtnComplete.interactable = false;
        }
    }

}
