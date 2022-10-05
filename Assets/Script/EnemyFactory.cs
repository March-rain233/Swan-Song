using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 敌人工厂
/// </summary>
public static class EnemyFactory
{
    /// <summary>
    /// 根据地图和描述创建敌人
    /// </summary>
    public static List<Unit> CreateEnemy(Map map, string description)
    {
        var enemies = new List<Unit>();
        enemies.Add(new Idiot());
        return enemies;
    }
}