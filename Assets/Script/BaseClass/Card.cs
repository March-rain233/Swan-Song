using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameToolKit;
using Newtonsoft.Json;

/// <summary>
/// 卡牌对象
/// </summary>
public abstract class Card
{
    /// <summary>
    /// 卡牌类型
    /// </summary>
    /// <remarks>
    /// 用于给AI判断卡牌类型来进行出牌逻辑
    /// </remarks>
    [Flags]
    public enum CardType
    {
        Other,
        Attack = 1,
        Defence = 1 << 1,
        Heal = 1 << 2,
    }

    /// <summary>
    /// 卡牌稀有度
    /// </summary>
    public enum CardRarity
    {
        Normal = 0,
        Privilege = 1,
    }

    /// <summary>
    /// 卡牌释放目标数据
    /// </summary>
    public struct TargetData
    {
        /// <summary>
        /// 可作为目标的图块坐标
        /// </summary>
        public IEnumerable<Vector2Int> AvaliableTile;
        /// <summary>
        /// 技能释放范围的坐标
        /// </summary>
        public IEnumerable<Vector2Int> ViewTiles;
    }
    /// <summary>
    /// 卡牌名称
    /// </summary>
    public string Name;

    /// <summary>
    /// 卡牌简介
    /// </summary>
    public string Description;

    /// <summary>
    /// 卡面图片
    /// </summary>
    [JsonConverter(typeof(SpriteConvert))]
    public Sprite Sprite;

    /// <summary>
    /// 卡牌的行动费用
    /// </summary>
    public int Cost;

    /// <summary>
    /// 是否已被强化
    /// </summary>
    public bool HasEnchanted;

    /// <summary>
    /// 卡牌类型
    /// </summary>
    [JsonIgnore]
    public virtual CardType Type => CardType.Other;

    public virtual CardRarity Rarity => CardRarity.Normal;

    /// <summary>
    /// 词条
    /// </summary>
    public HashSet<Entry> Entries = new();

    [JsonIgnore]
    protected static Map _map => (ServiceFactory.Instance.GetService<GameManager>().GetState() as BattleState).Map;

    /// <summary>
    /// 释放卡牌
    /// </summary>
    protected internal abstract void Release(Unit user, Vector2Int target);

    /// <summary>
    /// 获取可作用的目标
    /// </summary>
    /// <remarks>
    /// 即可以作为直接作用的目标
    /// </remarks>
    /// <returns></returns>
    protected internal abstract TargetData GetAvaliableTarget(Unit user);

    /// <summary>
    /// 获取影响范围
    /// </summary>
    /// <remarks>
    /// 即Aoe范围或是连锁单位等目标
    /// </remarks>
    /// <returns></returns>
    protected internal abstract IEnumerable<Vector2Int> GetAffecrTarget(Unit user, Vector2Int target);

    /// <summary>
    /// 复制卡牌对象
    /// </summary>
    public virtual Card Clone()
    {
        var card = MemberwiseClone() as Card;
        card.Entries = new HashSet<Entry>(card.Entries);
        return card;
    }

    /// <summary>
    /// 通用过滤器
    /// </summary>
    /// <param name="p"></param>
    /// <param name="isCheckUnit">是否过滤掉没有单位的单元格</param>
    /// <returns></returns>
    protected static bool UniversalFilter(Vector2Int p, bool isCheckUnit = false)
    {
        return 0 <= p.x && p.x < _map.Width
            && 0 <= p.y && p.y < _map.Height
            && _map[p] != null
            && (!isCheckUnit || (isCheckUnit & _map[p].Units.Count > 0));
    }

    /// <summary>
    /// 敌方单位过滤器
    /// </summary>
    /// <param name="p"></param>
    /// <param name="camp"></param>
    /// <returns>仅有非我方单位的格子</returns>
    protected static bool EnemyFilter(Vector2Int p, Camp camp)
    {
        return UniversalFilter(p, true) && _map[p].Units.First().Camp != camp;
    }

    /// <summary>
    /// 我方单位过滤器
    /// </summary>
    /// <param name="p"></param>
    /// <param name="camp"></param>
    /// <returns>排除我方单位所在单元格</returns>
    protected static bool ExcludeFriendFilter(Vector2Int p, Camp camp)
    {
        return UniversalFilter(p, false)
            && (_map[p].Units.Count > 0 ? _map[p].Units.First().Camp != camp : true);
    }

    /// <summary>
    /// 检测指定坐标图块能否放置单位
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    protected static bool CheckPlaceable(Vector2Int p, Unit unit)
    {
        return 0 <= p.x && p.x < _map.Width
            && 0 <= p.y && p.y < _map.Height
            && _map[p] != null
            && _map[p].CheckPlaceable(unit)
            && _map[p].Units.Count == 0;
    }

    /// <summary>
    /// 获取所有单位
    /// </summary>
    /// <param name="isIncluedDead"></param>
    /// <returns></returns>
    protected static IEnumerable<Unit> GetUnitList(bool isIncluedDead = false)
    {
        return GameManager.Instance.GetState<BattleState>()
            .UnitList.Where(u => isIncluedDead || u.ActionStatus != ActionStatus.Dead);
    }
}