using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 分支选择树
/// </summary>
public class TreeMap
{
    /// <summary>
    /// 节点查询表
    /// </summary>
    private Dictionary<int, TreeMapNode> _nodes;
    /// <summary>
    /// 边查询表
    /// </summary>
    private Dictionary<int, List<int>> _edges;
    private int _rootId;
    private int _currentId;

    /// <summary>
    /// 根节点
    /// </summary>
    public TreeMapNode Root
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 当前位于的节点
    /// </summary>
    public TreeMapNode CurrentNode
    {
        get => default;
        set
        {
        }
    }

    /// <summary>
    /// 获得指定节点的子节点
    /// </summary>
    public List<int> GetChildren(string id)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 根据Id查询节点
    /// </summary>
    public TreeMapNode FindNode(int id)
    {
        throw new System.NotImplementedException();
    }
}