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
    /// 数据包装器
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// 公式:(oriData + addValue) * Rate
    /// </remarks>
    public class DataWrapper
    {
        public int OriValue { get; internal set; } = 0;
        public int AddValue = 0;
        public float Rate = 1;
        public int Value => Mathf.FloorToInt((OriValue + AddValue) * Rate); 
    }
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
    public Sprite Face;

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
            DataChanged?.Invoke(this);
        }
    }
    int _actionPoint;

    /// <summary>
    /// 攻击力
    /// </summary>
    public int Attack => AttackWrapper.Value;
    public DataWrapper AttackWrapper { get; private set; }

    /// <summary>
    /// 当前单位的血量
    /// </summary>
    public int Blood
    {
        get => _blood;
        set
        {
            _blood = Mathf.Clamp(value, 0, BloodMax);
            DataChanged?.Invoke(this);
        }
    }
    int _blood;

    /// <summary>
    /// 防御力
    /// </summary>
    public int Defence => DefenceWrapper.Value;
    public DataWrapper DefenceWrapper { get; private set; }

    /// <summary>
    /// 治愈力
    /// </summary>
    public int Heal => HealWrapper.Value;
    public DataWrapper HealWrapper { get; private set; }

    /// <summary>
    /// 单位等级
    /// </summary>
    public int Level
    {
        get;
        private set;
    } = 1;

    /// <summary>
    /// 先手
    /// </summary>
    public int Speed => SpeedWrapper.Value;
    public DataWrapper SpeedWrapper { get; private set; }

    /// <summary>
    /// 血量上限
    /// </summary>
    public int BloodMax => BloodMaxWrapper.Value;
    public DataWrapper BloodMaxWrapper { get; private set; }

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

    public event Action<UnitData> DataChanged;
    public UnitData(UnitModel model)
    {
        UnitModel = model;
        Name = model.DefaultName;
        Face = model.DefaultFace;
        ViewType = model.DefaultViewType;
        BloodMaxWrapper.OriValue = Blood = model.Blood;
        AttackWrapper.OriValue = model.Attack;
        DefenceWrapper.OriValue = model.Defence;
        HealWrapper.OriValue = model.Heal;
        ActionPointMax = ActionPoint = model.ActionPoint;
        SpeedWrapper.OriValue = model.Speed;
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
        level = Mathf.Clamp(level, 1, GameManager.MaxLevel);
        //todo：升级公式
        Func<AnimationCurve, float, int> getDiff = (curve, baseValue) =>
         {
             int ori = Mathf.FloorToInt(curve.Evaluate(Level - 1) * baseValue);
             int next = Mathf.FloorToInt(curve.Evaluate(level - 1) * baseValue);
             return next - ori;
         };
        BloodMaxWrapper.OriValue += getDiff(UnitModel.BloodCurve, UnitModel.Blood);
        AttackWrapper.OriValue += getDiff(UnitModel.AttackCurve, UnitModel.Attack);
        DefenceWrapper.OriValue += getDiff(UnitModel.DefenceCurve, UnitModel.Defence);
        HealWrapper.OriValue += getDiff(UnitModel.HealCurve, UnitModel.Heal);
        SpeedWrapper.OriValue += getDiff(UnitModel.SpeedCurve, UnitModel.Speed);
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

        DataChanged?.Invoke(this);
    }

    public void RefreshBlood()
    {
        if(Blood > BloodMax)
        {
            Blood = BloodMax;
        }
    }
}