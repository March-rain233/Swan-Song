using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class TreeMapFactory
{
    /// <summary>
    /// 根据传入描述创建地图
    /// </summary>
    /// <param name="description">描述</param>
    public static TreeMap CreateTreeMap(string description)
    {
        //todo
        TreeMap map = new TreeMap();
        int start = map.AddNode(new TreeMapNodeData() { PlaceType = PlaceType.Start });
        map.Connect(start, map.AddNode(new TreeMapNodeData() { PlaceType = PlaceType.NormalBattle }));
        map.Connect(start, map.AddNode(new TreeMapNodeData() { PlaceType = PlaceType.BonFire }));
        return map;
    }
}