using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GameToolKit.Utility;
using System.Linq;

namespace GameToolKit.Dialog
{
    public static class RichTextHelper
    {
        /// <summary>
        /// 获取输出的文本
        /// </summary>
        /// <remarks>
        /// 文本将被过滤掉非unity原生的富文本标签
        /// </remarks>
        /// <returns></returns>
        public static string GetOutPutText(RichTextTree tree)
        {
            return GetNodeText(tree.RootNode);
        }

        /// <summary>
        /// 获取节点文本
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static string GetNodeText(RichTextNode node)
        {
            switch (node)
            {
                case PairTagNode pair:
                    var stringBuilder = new StringBuilder();
                    foreach (var child in pair.Children)
                    {
                        stringBuilder.Append(GetNodeText(child));
                    }
                    if (RichTextUtility.NativeTags.Contains(pair.Tag))
                    {
                        stringBuilder.Insert(0, $"<{pair.Attr}>");
                        stringBuilder.Append($"</{pair.Tag}>");
                    }
                    return stringBuilder.ToString();
                case EmptyTagNode:
                    return string.Empty;
                case RichTextNode text:
                    return text.ToString();
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 获取所有非unity原生的标签
        /// </summary>
        /// <returns></returns>
        public static List<TagNode> GetNonNativeTags(RichTextTree tree)
        {
            return tree.Where(n =>
            {
                var tag = n as TagNode;
                return n != null && !RichTextUtility.NativeTags.Contains(tag.Tag);
            }).ToList().ConvertAll(n => n as TagNode);
        }

        /// <summary>
        /// 获取包裹指定索引的文本的非原生标签对
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static List<(PairTagNode tag, int index)> GetNonNativeTagsOfIndex(RichTextTree tree, int index)
        {
            if(index >= tree.RootNode.PlainTextLength)
            {
                return null;
            }

            //todp:sprite标签是潜在bug，未来如果遇到需要修复
            List<(PairTagNode tag, int index)> tags = new List<(PairTagNode tag, int index)>();

            PairTagNode p = tree.RootNode;
            int offset = index;
            //查找指定索引位于标签对的哪个孩子
            while (p != null)
            {
                int cum = 0;
                foreach(var child in p.Children)
                {
                    int l = child.PlainTextLength;
                    cum += l;
                    if (cum > offset)
                    {
                        offset = offset + l - cum;
                        tags.Add((p, offset));
                        p = child as PairTagNode;
                        break;
                    }
                }
            }
            return tags;
        }

        /// <summary>
        /// 获取遍历过程中每个索引拥有的标签
        /// </summary>
        /// <param name="tree"></param>
        /// <returns>开始标签，结束标签，空白标签组</returns>
        public static IEnumerator<(Queue<PairTagNode> begin, Queue<PairTagNode> end, Queue<EmptyTagNode> emptys)> GetTagInTraver(RichTextTree tree)
        {
            Queue<PairTagNode> begin = new Queue<PairTagNode>();
            Queue<PairTagNode> end = new Queue<PairTagNode>();
            Queue<EmptyTagNode> emptys = new Queue<EmptyTagNode>();
            System.Action<RichTextNode> getEnd = (node) =>
            {
                if (node != null && node.NextSibling == null)
                {
                    end.Enqueue(node.Parent);
                    node = node.Parent;
                }
            };
            foreach (var child in tree)
            {
                switch (child)
                {
                    case EmptyTagNode empty:
                        emptys.Enqueue(empty);
                        getEnd(child);
                        break;
                    case PairTagNode pair:
                        begin.Enqueue(pair);
                        break;
                    case InnerTextNode text:
                        if (emptys.Count > 0)
                        {
                            yield return (begin, end, emptys);
                            begin.Clear();
                            end.Clear();
                            emptys.Clear();
                        }
                        for (int i = 0; i < text.Text.Length; i++)
                        {
                            if (i == text.Text.Length - 1)
                            {
                                getEnd(child);
                            }
                            yield return (begin, end, emptys);
                            begin.Clear();
                            end.Clear();
                            emptys.Clear();
                        }
                        break;
                }
            }
            if (emptys.Count > 0)
            {
                yield return (begin, end, emptys);
                begin.Clear();
                end.Clear();
                emptys.Clear();
            }
        }
    }
}
