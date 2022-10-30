using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Text.RegularExpressions;

namespace GameToolKit
{
    public class CustomGraph<TNode> : SerializedScriptableObject where TNode : BaseNode
    {
        /// <summary>
        /// 节点列表
        /// </summary>
        public List<TNode> Nodes = new List<TNode>();
        /// <summary>
        /// 根据guid查找节点
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public TNode FindNode(string guid)
        {
            return Nodes.Find(n => n.Guid == guid);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        public virtual void RemoveNode(TNode node)
        {
            Nodes.Remove(node);
            //移除以该节点的资源边
            foreach (var child in Nodes)
            {
                for (int i = child.InputEdges.Count - 1; i >= 0; --i)
                    if (child.InputEdges[i].SourceNode == node)
                        child.InputEdges.RemoveAt(i);
                for (int i = child.OutputEdges.Count - 1; i >= 0; --i)
                    if (child.OutputEdges[i].TargetNode == node)
                        child.OutputEdges.RemoveAt(i);
            }
        }
        /// <summary>
        /// 根据类型创建节点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual TNode CreateNode(Type type)
        {

            //创建节点
            var node = Activator.CreateInstance(type) as TNode;

            //生成节点名
            string newName;
            var attr = type.GetCustomAttributes(typeof(NodeNameAttribute), true);
            if(attr.Length > 0)
            {
                newName = (attr[0] as NodeNameAttribute).Name;
            }
            else
            {
                newName = type.Name;
                if (type.IsGenericType)
                {
                    newName = newName.Remove(newName.Length - 2);
                }
            }
#if UNITY_EDITOR
            //计算节点名的使用次数
            int count = Nodes.FindAll(node => Regex.IsMatch(node.Name, @$"{newName}(\(\d+\))?$")).Count;
            if (count > 0)
            {
                newName = newName + $"({count})";
            }
            node.Name = newName;
#endif
            //生成节点的唯一标识符
            node.Guid = Guid.NewGuid().ToString();
            Nodes.Add(node);
            return node;
        }
    }
}