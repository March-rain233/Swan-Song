using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

/// <summary>
/// 分支选择树
/// </summary>
[Serializable]
public class TreeMap
{
    /// <summary>
    /// 节点查询表
    /// </summary>
    [JsonRequired]
    private Dictionary<int, TreeMapNodeData> _nodes = new();
    /// <summary>
    /// 边查询表
    /// </summary>
    [JsonRequired]
    private Dictionary<int, HashSet<int>> _edges = new();

    [JsonIgnore]
    public int RootId = 0;

    /// <summary>
    /// 当前节点的ID
    /// </summary>
    [JsonIgnore]
    public int CurrentId
    {
        get => _path.Last();
        set => _path.Add(value);
    }

    [JsonProperty]
    private List<int> _path = new List<int>();

    [JsonIgnore]
    public IReadOnlyCollection<int> Path => _path;

    /// <summary>
    /// 节点ID列表
    /// </summary>
    [JsonIgnore]
    public IEnumerable<int> Nodes => _nodes.Keys;
    /// <summary>
    /// 根节点
    /// </summary>
    [JsonIgnore]
    public TreeMapNodeData Root => FindNode(RootId);

    /// <summary>
    /// 当前位于的节点
    /// </summary>
    [JsonIgnore]
    public TreeMapNodeData CurrentNode => FindNode(CurrentId);

    /// <summary>
    /// 判断是否有子节点
    /// </summary>
    public bool HavingChildren(int id)
    {
        return _edges.ContainsKey(id);
    }

    /// <summary>
    /// 获得指定节点的子节点
    /// </summary>
    public HashSet<int> GetChildren(int id)
    {
        if (_edges.ContainsKey(id))
        {
            return _edges[id];
        }
        return new HashSet<int>();
    }

    /// <summary>
    /// 根据Id查询节点
    /// </summary>
    public TreeMapNodeData FindNode(int id)
    {
        return _nodes[id];
    }
    
    /// <summary>
    /// 增加节点
    /// </summary>
    /// <param name="node"></param>
    /// <returns>节点ID</returns>
    public int AddNode(TreeMapNodeData node)
    {
        _nodes.Add(_nodes.Count, node);
        return _nodes.Count - 1;
    }

    /// <summary>
    /// 连接节点
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public bool Connect(int start, int end)
    {
        if(!_edges.ContainsKey(start))
        {
            _edges.Add(start, new());
        }
        return _edges[start].Add(end);
    }
}