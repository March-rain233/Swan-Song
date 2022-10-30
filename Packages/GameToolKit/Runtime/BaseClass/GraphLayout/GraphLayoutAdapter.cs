using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Utility
{
    /// <summary>
    /// 自动布局图表适配器
    /// </summary>
    public abstract class GraphLayoutAdapter
    {
        /// <summary>
        /// 参与布局的所有节点
        /// </summary>
        public abstract IEnumerable<int> Nodes { get; }

        /// <summary>
        /// 获取指定节点的后继节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IEnumerable<int> GetDescendant(int id);

        /// <summary>
        /// 获取指定节点的前驱节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IEnumerable<int> GetPrecursor(int id);

        /// <summary>
        /// 获取指定节点的节点数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract NodeData GetNodeData(int id);

        /// <summary>
        /// 设定指定节点的节点数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public abstract void SetNodeData(int id, NodeData data);
    }

    /// <summary>
    /// 获取节点数据
    /// </summary>
    public struct NodeData
    {
        public float Width;
        public float Height;
        public Vector2 Position;
    }
}
