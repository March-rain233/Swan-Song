using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 单位数据成员
/// </summary>
public class UnitData
{
    /// <summary>
    /// 单位显示类型
    /// </summary>
    public int ViewType = 0;

    /// <summary>
    /// 单位名字
    /// </summary>
    public string Name;

    /// <summary>
    /// 单位头像
    /// </summary>
    public UnityEngine.Sprite Face;

    /// <summary>
    /// 行动点上限
    /// </summary>
    public int ActionPointMax;

    /// <summary>
    /// 行动点
    /// </summary>
    public int ActionPoint
    {
        get => _actionPoint;
        set
        {
            _actionPoint = value;
            DataChanged?.Invoke();
        }
    }
    int _actionPoint;

    /// <summary>
    /// 攻击力
    /// </summary>
    public int Attack;

    /// <summary>
    /// 当前单位的血量
    /// </summary>
    public int Blood
    {
        get => _blood;
        set
        {
            _blood = value;
            DataChanged?.Invoke();
        }
    }
    int _blood;

    /// <summary>
    /// 防御力
    /// </summary>
    public int Defence;

    /// <summary>
    /// 治愈力
    /// </summary>
    public int Heal;

    /// <summary>
    /// 单位等级
    /// </summary>
    public int Level = 1;

    /// <summary>
    /// 先手
    /// </summary>
    public int Speed;

    /// <summary>
    /// 血量上限
    /// </summary>
    public int BloodMax;

    public UnitModel UnitModel
    {
        get;
        internal set;
    }

    /// <summary>
    /// 持有的牌库
    /// </summary>
    public List<Card> Deck
    {
        get;
        internal set;
    }

    public event Action DataChanged;

    public UnitData() { }
    public UnitData(UnitModel model)
    {
        UnitModel = model;
        Name = model.DefaultName;
        Face = model.DefaultFace;
        ViewType = model.DefaultViewType;
        BloodMax = Blood = model.Blood;
        Attack = model.Attack;
        Defence = model.Defence;
        Heal = model.Heal;
        ActionPointMax = ActionPoint = model.ActionPoint;
        Speed = model.Speed;
        Deck = new();
        foreach(var card in model.DefaultDeck)
        {
            Deck.Add(card.Clone());
        }
    }

    /// <summary>
    /// 复制单位数据
    /// </summary>
    public UnitData Clone()
    {
        return MemberwiseClone() as UnitData;
    }

    /// <summary>
    /// 升级单位
    /// </summary>
    public void LevelUp()
    {
        SetLevel(Level + 1);
    }

    public void SetLevel(int level)
    {
        //todo：升级公式
        Func<AnimationCurve, float, int> getDiff = (curve, baseValue) =>
         {
             int ori = Mathf.FloorToInt(curve.Evaluate(Level - 1) * baseValue);
             int next = Mathf.FloorToInt(curve.Evaluate(level - 1) * baseValue);
             return next - ori;
         };
        BloodMax += getDiff(UnitModel.BloodCurve, UnitModel.Blood);
        Attack += getDiff(UnitModel.AttackCurve, UnitModel.Attack);
        Defence += getDiff(UnitModel.DefenceCurve, UnitModel.Defence);
        Heal += getDiff(UnitModel.HealCurve, UnitModel.Heal);
        Speed += getDiff(UnitModel.SpeedCurve, UnitModel.Speed);
        ActionPointMax += getDiff(UnitModel.ActionPointCurve, UnitModel.ActionPoint);

        if(level > Level)
        {
            Blood = BloodMax;
            ActionPoint = ActionPointMax;
        }
        else
        {
            Blood = Mathf.Min(BloodMax, Blood);
            ActionPoint = Mathf.Min(ActionPointMax, ActionPoint);
        }
        Level = level;
    }
}