using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// 玩家队伍选择界面
/// </summary>
public class PlayerSelectPanel : PanelBase
{
    List<UnitModel> _team = new();

    UnitData _currentData;
    UnitModel _currentModel;

    List<(Button btn, Outline ol, UnitModel model)> SelectList = new();

    public HorizontalLayoutGroup UnitList;

    public TextMeshProUGUI Blood;
    public TextMeshProUGUI Attack;
    public TextMeshProUGUI Defence;
    public TextMeshProUGUI Heal;
    public TextMeshProUGUI Speed;
    public TextMeshProUGUI AP;
    public TextMeshProUGUI Level;
    public Button BtnLevelUp;
    public Button BtnLevelDown;
    public TextMeshProUGUI Name;
    public Image Face;
    public Button BtnSkill;

    public Button BtnComplete;
    public Button BtnJoin;
    public Button BtnDelete;

    public List<Image> TeamView;
    protected override void OnInit()
    {
        base.OnInit();
        foreach (var unitModel in UnitSetting.Instance.PlayerList)
        {
            var background = new GameObject(unitModel.DefaultName, 
                typeof(RectTransform), typeof(Image), typeof(Outline));
            var obj = new GameObject(unitModel.DefaultName, 
                typeof(RectTransform),typeof(Image), typeof(Button));
            background.transform.SetParent(UnitList.transform, false);
            obj.transform.SetParent(background.transform, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            var btn = obj.GetComponent<Button>();
            var img = obj.GetComponent<Image>();
            var ol = background.GetComponent<Outline>();
            btn.image = img;
            img.sprite = unitModel.DefaultFace;
            img.preserveAspect = true;
            ol.effectDistance = new Vector2(3, 3);
            ol.effectColor = Color.blue;
            ol.enabled = false;
            btn.onClick.AddListener(() =>
            {
                if(_currentModel != unitModel)
                {
                    _currentModel = unitModel;
                    _currentData = new UnitData(unitModel);
                    Name.text = _currentData.Name;
                    Face.sprite = _currentData.Face;
                    UpdataView();

                    BtnLevelDown.interactable = false;
                    BtnLevelUp.interactable = true;

                    BtnJoin.interactable = !_team.Contains(unitModel) && _team.Count < TeamView.Count;
                    BtnDelete.interactable = _team.Contains(unitModel);
                }
            });

            SelectList.Add((btn, ol, unitModel));
        }
        BtnLevelDown.onClick.AddListener(() =>
        {
            _currentData.SetLevel(_currentData.Level - 1);
            if(_currentData.Level == 1)
            {
                BtnLevelDown.interactable = false;
            }
            UpdataView();
        });
        BtnLevelUp.onClick.AddListener(() =>
        {
            _currentData.SetLevel(_currentData.Level + 1);
            BtnLevelDown.interactable = true;
            UpdataView();
        });

        BtnComplete.interactable = false;
        BtnJoin.onClick.AddListener(() =>
        {
            _team.Add(_currentModel);
            TeamView[_team.Count - 1].sprite = _currentModel.DefaultFace;
            BtnComplete.interactable = true;
            BtnDelete.interactable = true;
            BtnJoin.interactable = false;

            SelectList.Find(p => p.model == _currentModel).ol.enabled = true;
        });
        BtnDelete.onClick.AddListener(() =>
        {
            _team.Remove(_currentModel);
            for(int i = 0; i < _team.Count; ++i)
            {
                TeamView[i].sprite = _team[i].DefaultFace;
            }
            for(int i = _team.Count; i < TeamView.Count; ++i)
            {
                TeamView[i].sprite = null;
            }
            BtnComplete.interactable = _team.Count > 0;
            BtnDelete.interactable = false;
            BtnJoin.interactable = true;

            SelectList.Find(p => p.model == _currentModel).ol.enabled = false;
        });
        BtnComplete.onClick.AddListener(() =>
        {
            GameManager.Instance.GetState<PlayerSelectState>().SetTeam(_team);
        });

        SelectList[0].btn.onClick.Invoke();
    }

    void UpdataView()
    {
        Blood.text = $"{_currentData.BloodMax}";
        Attack.text = $"{_currentData.Attack}";
        Defence.text = $"{_currentData.Defence}";
        Speed.text = $"{_currentData.Speed}";
        Heal.text = $"{_currentData.Heal}";
        AP.text = $"{_currentData.ActionPointMax}";
        Level.text = $"Level {_currentData.Level}";
    }
}
