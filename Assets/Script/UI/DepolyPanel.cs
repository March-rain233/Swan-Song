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

    GameObject _toggleModel => UISetting.Instance.PrefabsDic["UnitSelectView"];

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
        var gm = ServiceFactory.Instance.GetService<GameManager>();
        var mr = ServiceFactory.Instance.GetService<MapRenderer>();
        var state = gm.GetState() as BattleState;
        state.DeployBeginning -= DepolyPanel_DeployBeginning;
        mr.MaskTilemap.ClearAllTiles();
    }

    private void DepolyPanel_DeployBeginning(List<Vector2Int> arg1, List<UnitData> arg2)
    {
        _points = arg1;
        _units = arg2;
        var mr = ServiceFactory.Instance.GetService<MapRenderer>();
        foreach (var p in arg1)
        {
            mr.MaskTilemap.SetTile(new Vector3Int(p.x, p.y), mr.SquareTile);
            mr.MaskTilemap.SetColor(new Vector3Int(p.x, p.y), Color.green);
        }
        foreach (var p in arg2)
        {
            CreateSelect(p);
        }

        CurrentSelect.Toggle.onValueChanged.Invoke(true);
    }

    void CreateSelect(UnitData unit)
    {
        var model = Instantiate(_toggleModel, UnitSelectGroup.transform);
        var view = model.GetComponent<UnitSelectView>();
        view.Binding(unit);
        view.Toggle.group = UnitSelectGroup;
    }

    private void Update()
    {
        var mr = ServiceFactory.Instance.GetService<MapRenderer>();
        var mouse = TileUtility.GetMouseInCell();
        mouse.z = 0;
        var pos = new Vector2Int(mouse.x, mouse.y);
        //�������˿�ѡ���ͼ�飬�Ҹ�ͼ�黹δ����λռ�ݣ��򽫵�ǰѡ�е�λ�����ڸ�ͼ��
        //�����ͼ��ռ�ݵĵ�λ���ǵ�ǰ��λ����ȡ����λ��ս״̬
        if (Mouse.current.leftButton.wasReleasedThisFrame && _points.Contains(pos))
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
        unitView.transform.position = ServiceFactory.Instance.GetService<MapRenderer>()
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
