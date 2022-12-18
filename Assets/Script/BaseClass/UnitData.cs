using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

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
        public int OriValue = 0;
        public int AddValue = 0;
        public float Rate = 1;
        [JsonIgnore]
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
    /// 单位描述
    /// </summary>
    public string Description;

    /// <summary>
    /// 单位头像
    /// </summary>
    [JsonConverter(typeof(ObjectConvert))]
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
            _actionPoint = Mathf.Max(0, value);
            DataChanged?.Invoke(this);
        }
    }
    int _actionPoint;

    /// <summary>
    /// 攻击力
    /// </summary>
    [JsonIgnore]
    public int Attack => AttackWrapper.Value;
    [JsonProperty]
    public DataWrapper AttackWrapper { get; private set; } = new DataWrapper();

    /// <summary>
    /// 防御力
    /// </summary>
    [JsonIgnore]
    public int Defence => DefenceWrapper.Value;
    [JsonProperty]
    public DataWrapper DefenceWrapper { get; private set; } = new DataWrapper();

    /// <summary>
    /// 治愈力
    /// </summary>
    [JsonIgnore]
    public int Heal => HealWrapper.Value;
    [JsonProperty]
    public DataWrapper HealWrapper { get; private set; } = new DataWrapper();

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
    [JsonIgnore]
    public int Speed => SpeedWrapper.Value;
    [JsonProperty]
    public DataWrapper SpeedWrapper { get; private set; } = new DataWrapper();

    /// <summary>
    /// 血量上限
    /// </summary>
    [JsonIgnore]
    public int BloodMax => BloodMaxWrapper.Value;
    [JsonProperty]
    public DataWrapper BloodMaxWrapper { get; private set; } = new DataWrapper();

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

    public UnitModel UnitModel { get; private set; }

    /// <summary>
    /// 持有的牌库
    /// </summary>
    public List<Card> Deck;
    /// <summary>
    /// 套牌索引
    /// </summary>
    public List<int> BagIndex = new List<int>();

    /// <summary>
    /// 套牌
    /// </summary>
    [JsonIgnore]
    public IEnumerable<Card> Bag => BagIndex.Select(i => Deck[i]);

    public event Action<UnitData> DataChanged;

    [JsonConstructor]
    public UnitData(UnitModel unitModel)
    {
        UnitModel = unitModel;
        Name = unitModel.DefaultName;
        Description = unitModel.DefaultDescription;
        Face = unitModel.DefaultFace;
        ViewType = unitModel.DefaultViewType;
        BloodMaxWrapper.OriValue = unitModel.Blood;
        Blood = BloodMax;
        AttackWrapper.OriValue = unitModel.Attack;
        DefenceWrapper.OriValue = unitModel.Defence;
        HealWrapper.OriValue = unitModel.Heal;
        ActionPointMax = ActionPoint = unitModel.ActionPoint;
        SpeedWrapper.OriValue = unitModel.Speed;
        Deck = new();
        foreach(var card in unitModel.DefaultDeck)
        {
            Deck.Add(card.Clone());
        }
        BagIndex = Enumerable.Range(0, Mathf.Min(20, Deck.Count)).ToList();
    }

    //public UnitData()
    //{

    //}

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