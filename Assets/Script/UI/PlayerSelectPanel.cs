using System.Collections;
using System.Linq;
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

    UnitModel _currentModel;

    List<(Button btn, Outline ol, UnitModel model)> SelectList = new();

    public HorizontalLayoutGroup UnitList;

    public Button BtnComplete;
    public Button BtnJoin;
    public Button BtnDelete;

    public UnitDataDetailView UnitDataView;

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
                    UnitDataView.UnitData = new UnitData(unitModel);
                    UnitDataView.Refresh();

                    UnitDataView.BtnLevelDown.interactable = false;
                    UnitDataView.BtnLevelUp.interactable = true;

                    BtnJoin.interactable = !_team.Contains(unitModel) && _team.Count < TeamView.Count;
                    BtnDelete.interactable = _team.Contains(unitModel);
                }
            });

            SelectList.Add((btn, ol, unitModel));
        }

        UnitDataView.BtnSkill.onClick.AddListener(() =>
        {
            var data = UnitDataView.UnitData;
            var res = new List<(string, IEnumerable<(Card, UnitData)>)>();
            System.Func<string, string, (string, IEnumerable<(Card, UnitData)>)> func = (index, name) =>
            {
                return (name, from card in CardPoolManager.Instance.PoolDic[index].Where(c=>c.Rarity == Card.CardRarity.Privilege)
                              select (card, data));
            };
            res.Add(func(data.UnitModel.PrivilegeDeckIndex, "专属"));
            var panel = ServiceFactory.Instance.GetService<PanelManager>()
                .OpenPanel(nameof(DeckPanel)) as DeckPanel;
            panel.ShowCardList(res);
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
}
