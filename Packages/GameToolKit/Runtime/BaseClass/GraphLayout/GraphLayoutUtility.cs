using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameToolKit.Utility
{
    public class GraphLayoutUtility
    {
        /// <summary>
        /// 多叉树布局
        /// </summary>
        /// <remarks>
        /// 要求：无环
        /// </remarks>
        public static void TreeLayout(GraphLayoutAdapter graph, int root,
            Vector2 oriPosition = default, float marginWidth = 20f, float marginHeight = 20f)
        {
            Dictionary<int, Rect> treeRects = new();
            HashSet<int> visited = new();
            //生成子树的矩形大小
            var rectWalker = LambdaUtility.Fix<int, Rect>(f => id =>
            {
                visited.Add(id);
                var node = graph.GetNodeData(id);
                Rect rect = Rect.zero;
                var children = graph.GetDescendant(id);
                foreach (int child in children)
                {
                    var cr = f(child);
                    rect.width = Mathf.Max(cr.width, rect.width);
                    rect.height += cr.height;
                }
                rect.height += marginHeight * (children.Count() - 1);
                rect.width += children.Count() > 0 ? marginWidth : 0;
                rect.width += node.Width;
                rect.height = Mathf.Max(node.Height, rect.height);
                treeRects.Add(id, rect);
                return rect;
            });
            rectWalker(root);
            visited.Clear();
            //设置子树位置
            if(oriPosition == default)
            {
                oriPosition = Vector2.zero;
            }
            var positionWalker = LambdaUtility.Fix<int>(f => id =>
            {
                var data = graph.GetNodeData(id);
                data.Position = treeRects[id].position + new Vector2(data.Width /2, data.Height/2);
                graph.SetNodeData(id, data);
                Vector2 pos = treeRects[id].position + new Vector2(data.Width + marginWidth, -treeRects[id].height / 2);
                foreach (var child in graph.GetDescendant(id))
                {
                    var temp = treeRects[child];
                    temp.position += pos + new Vector2(0, treeRects[child].height / 2);
                    treeRects[child] = temp;
                    pos.y += treeRects[child].height + marginHeight;
                    f(child);
                }
            });
            positionWalker(root);
            visited.Clear();
        }

        /// <summary>
        /// 层级布局
        /// </summary>
        /// <remarks>
        /// 要求：无环
        /// </remarks>
        public static void HierarchicalLayout(GraphLayoutAdapter graph, int root, 
            out float actualWidth, out float actualHeight,
            Vector2 oriPosition = default, float intervalWidth = 20f, float intervalHeight = 20f)
        {
            Dictionary<int, int> levels = new();
            HashSet<int> visited = new();
            actualHeight = actualWidth = 0;
            //确定节点层级
            System.Action<int> walker = (int start) =>
            {
                Queue<int> queue = new();
                Queue<int> buffer = new();
                queue.Enqueue(start);
                int level = 0;
                while(queue.Count > 0 || buffer.Count > 0)
                {
                    var id = queue.Dequeue();
                    visited.Add(id);
                    if (levels.ContainsKey(id))
                    {
                        levels[id] = System.Math.Max(level, levels[id]);
                    }
                    else
                    {
                        levels.Add(id, level);
                    }
                    foreach(var child in graph.GetDescendant(id))
                    {
                        buffer.Enqueue(child);
                    }
                    if(queue.Count == 0)
                    {
                        var temp = buffer;
                        buffer = queue;
                        queue = temp;
                        level += 1;
                    }
                }
            };
            walker(root);
            foreach(var n in graph.Nodes)
            {
                if (!visited.Contains(n))
                {
                    walker(n);
                }
            }
            //设定节点位置
            int max = levels.Max(p=>p.Value);
            float x = 0;
            for(int i = 0; i <= max; i++)
            {
                float maxWidth = 0;
                float height = 0;
                foreach(var n in levels.Where(n=>n.Value == i).Select(p=>p.Key))
                {
                    var data = graph.GetNodeData(n);
                    maxWidth = System.Math.Max(maxWidth, data.Width);
                    height += data.Height;
                }
                height += intervalHeight * System.Math.Max(0, levels.Count(n => n.Value == i) - 1);
                float y = -height / 2;
                foreach (var n in levels.Where(n => n.Value == i).Select(p => p.Key))
                {
                    var data = graph.GetNodeData(n);
                    data.Position = new Vector2(x + data.Width/2, y + data.Height/2);
                    graph.SetNodeData(n, data);
                    y += data.Height + intervalHeight;
                }
                x += intervalWidth + maxWidth;
                actualHeight = Mathf.Max(actualHeight, height);
            }
            actualWidth = x - intervalWidth;
            
        }
    }
}
