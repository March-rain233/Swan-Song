using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 单位数据成员
/// </summary>
public class UnitData
{
    /// <summary>
    /// 行动点上限
    /// </summary>
    public int ActionPointMax;

    /// <summary>
    /// 攻击力
    /// </summary>
    public int Attack;

    /// <summary>
    /// 当前单位的血量
    /// </summary>
    public int Blood;

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
    public int Level;

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

    /// <summary>
    /// 人物ID
    /// </summary>
    public int ID
    {
        get;
        private set;
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
        //todo：升级公式
        throw new System.NotImplementedException();
    }
}