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
    public int ActionPointMax
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 攻击力
    /// </summary>
    public int Attack
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 当前单位的血量
    /// </summary>
    public int Blood
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 防御力
    /// </summary>
    public int Defence
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 治愈力
    /// </summary>
    public int Heal
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 单位等级
    /// </summary>
    public int Level
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 先手
    /// </summary>
    public int Speed
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 血量上限
    /// </summary>
    public int BloodMax
    {
        get => default;
        set
        {
        }
    }

    public UnitModel UnitModel
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 持有的牌库
    /// </summary>
    public List<Card> Deck
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 人物ID
    /// </summary>
    public int ID
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 复制单位数据
    /// </summary>
    public UnitData Clone()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 升级单位
    /// </summary>
    public void LevelUp()
    {
        throw new System.NotImplementedException();
    }
}