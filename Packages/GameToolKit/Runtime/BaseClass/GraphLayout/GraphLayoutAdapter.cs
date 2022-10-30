using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Utility
{
    /// <summary>
    /// �Զ�����ͼ��������
    /// </summary>
    public abstract class GraphLayoutAdapter
    {
        /// <summary>
        /// ���벼�ֵ����нڵ�
        /// </summary>
        public abstract IEnumerable<int> Nodes { get; }

        /// <summary>
        /// ��ȡָ���ڵ�ĺ�̽ڵ�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IEnumerable<int> GetDescendant(int id);

        /// <summary>
        /// ��ȡָ���ڵ��ǰ���ڵ�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IEnumerable<int> GetPrecursor(int id);

        /// <summary>
        /// ��ȡָ���ڵ�Ľڵ�����
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract NodeData GetNodeData(int id);

        /// <summary>
        /// �趨ָ���ڵ�Ľڵ�����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public abstract void SetNodeData(int id, NodeData data);
    }

    /// <summary>
    /// ��ȡ�ڵ�����
    /// </summary>
    public struct NodeData
    {
        public float Width;
        public float Height;
        public Vector2 Position;
    }
}
