using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using System.Collections.ObjectModel;
using GameToolKit.Utility;
namespace GameToolKit.Dialog
{
    /// <summary>
    /// 富文本解析树
    /// </summary>
    public class RichTextTree : IEnumerable<RichTextNode>
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public RootNode RootNode { get; private set; }

        /// <summary>
        /// 根据传入文本创建解析树
        /// </summary>
        /// <param name="rawText"></param>
        public RichTextTree(string rawText)
        {
            RootNode = new RootNode();
            Stack<PairTagNode> stack = new Stack<PairTagNode>();
            Dictionary<string, bool> isEmptyTag = new Dictionary<string, bool>();//判断是否是空标签
            StringBuilder stringBuilder = new StringBuilder(rawText);
            stack.Push(RootNode);
            Action<string> createTextNode = (string body) =>
            {
                RichTextNode node = new InnerTextNode(body);
                stack.Peek().AddChild(node);
                stringBuilder.Remove(0, body.Length);
            };

            //todo：处理<noparse></noparse>组
            while (stringBuilder.Length > 0)
            {
                var temp = stringBuilder.ToString();
                var match = Regex.Match(temp, "<([^\"<>]|\".*\")*?>");//查找下一标签
                if (match.Success)
                {
                    if (match.Index != 0)
                    {
                        createTextNode(temp.Substring(0, match.Index));
                    }

                    string tag = match.Value.Substring(1, match.Length - 2);
                    if (tag[0] == '/')//如果是闭标签弹出栈顶元素
                    {
                        var node = stack.Pop();
                        if (node.Tag != tag.Remove(0, 1))
                        {
                            throw new RichTextTreeException(node, $"{tag.Remove(0, 1)} Labels do not match");
                        }
                        stringBuilder.Remove(0, match.Length);
                    }
                    else
                    {
                        //获取标签对应类型
                        var tagName = RichTextUtility.GetTagType(tag);
                        //判断是否是空标签
                        if (!isEmptyTag.ContainsKey(tagName))
                        {
                            isEmptyTag[tagName] = !rawText.Contains($"</{tagName}>");
                        }
                        if (isEmptyTag[tagName])
                        {
                            var node = new EmptyTagNode(tagName, tag);
                            stack.Peek().AddChild(node);
                        }
                        else
                        {
                            //如果是开标签则压入元素
                            var node = new PairTagNode(tagName, tag);
                            stack.Peek().AddChild(node);
                            stack.Push(node);
                        }
                        stringBuilder.Remove(0, match.Length);
                    }
                }
                else
                {
                    createTextNode(temp);
                    break;
                }
            }

            if (stack.Peek() != RootNode)
            {
                throw new RichTextTreeException(RootNode, "Parse failure");
            }
        }

        /// <summary>
        /// 插入纯文本
        /// </summary>
        /// <param name="index">纯文本插入索引</param>
        /// <param name="text"></param>
        public void InsertPlainText(int index, string text)
        {
            InnerTextNode node = null;
            foreach (InnerTextNode child in this.Where(t => t is InnerTextNode))
            {
                if (index - child.Length <= 0)
                {
                    node = child;
                    break;
                }
                index -= child.Length;
            }
            if (node != null)
            {
                node.Insert(index, text);
            }
            else
            {
                RootNode.AddChild(new InnerTextNode(text));
            }
        }

        /// <summary>
        /// 移除纯文本
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        public void RemovePlainText(int startIndex, int endIndex)
        {
            //获取需要删除的元素
            var index = 0;
            List<(InnerTextNode node, int index, int length)> textList = new List<(InnerTextNode, int, int)>();
            List<EmptyTagNode> emptyTagList = new List<EmptyTagNode>();
            foreach (var child in this)
            {
                switch (child)
                {
                    case InnerTextNode text:
                        index += text.Length;
                        if (index > startIndex)
                        {
                            int length = Math.Min(Math.Min(index, endIndex) - startIndex, text.Length);
                            int start = Math.Max(startIndex + text.Length - index, 0);
                            textList.Add((text, start, length));
                        }
                        break;
                    case EmptyTagNode emptyTag:
                        emptyTagList.Add(emptyTag);
                        break;
                }
                if (index >= endIndex)
                {
                    break;
                }
            }

            //删除元素
            foreach ((var n, var i, var l) in textList)
            {
                n.Remove(i, l);
            }
            foreach (var n in emptyTagList)
            {
                n.Parent.RemoveChild(n);
            }
        }

        /// <summary>
        /// 替换纯文本
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="replaceText"></param>
        public void ReplacePlainText(int startIndex, int endIndex, string replaceText)
        {
            //获取需要删除的元素
            var index = 0;
            List<(InnerTextNode node, int index, int length)> textList = new List<(InnerTextNode, int, int)>();
            List<EmptyTagNode> emptyTagList = new List<EmptyTagNode>();
            foreach (var child in this)
            {
                switch (child)
                {
                    case InnerTextNode text:
                        index += text.Length;
                        if (index > startIndex)
                        {
                            int length = Math.Min(Math.Min(index, endIndex) - startIndex, text.Length);
                            int temp = Math.Max(startIndex + text.Length - index, 0);
                            textList.Add((text, temp, length));
                        }
                        break;
                    case EmptyTagNode emptyTag:
                        emptyTagList.Add(emptyTag);
                        break;
                }
                if (index >= endIndex)
                {
                    break;
                }
            }

            //删除除了第一个文本元素外的所有元素
            var first = textList.First();
            textList.RemoveAt(0);
            foreach ((var n, var i, var l) in textList)
            {
                n.Remove(i, l);
            }
            foreach (var n in emptyTagList)
            {
                n.Parent.RemoveChild(n);
            }

            //将替换文本插入第一个文本元素后删除原先的文本
            (var textNode, var start, var len) = first;
            textNode.Insert(textNode.Length, replaceText);
            textNode.Remove(start, len);
        }

        /// <summary>
        /// 获取范围内指定的标签
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public List<PairTagNode> GetPairTagInRange(int startIndex, int endIndex, string tag)
        {
            var list = new HashSet<PairTagNode>();
            var index = 0;
            foreach (var child in this)
            {
                if (child is InnerTextNode)
                {
                    index += child.Length;
                }
                if (child is not PairTagNode)
                {
                    if (index >= startIndex)
                    {
                        list.Add(child.Parent);
                    }
                    if (index >= endIndex)
                    {
                        break;
                    }
                }
            }

            var pairList = new HashSet<PairTagNode>();
            foreach (var elem in list)
            {
                var p = elem;
                while (p != null)
                {
                    if (p.Tag == tag)
                    {
                        pairList.Add(p);
                        break;
                    }
                    p = p.Parent;
                }
            }
            return pairList.ToList();
        }

        /// <summary>
        /// 设置范围内的成对标签
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="tag"></param>
        /// <param name="attr"></param>
        public void SetPairTag(int startIndex, int endIndex, string tag, string attr)
        {
            (var nodes, var start, var end) = GetNodeBetweenIndex(startIndex, endIndex);

            //修剪尾部文本
            (nodes.Last(t => t is InnerTextNode) as InnerTextNode).Spilt(end);

            //修剪头部文本
            var temp = nodes.First(t => t is InnerTextNode) as InnerTextNode;
            var buffer = temp.Spilt(start);
            var index = nodes.IndexOf(temp);
            nodes.RemoveAt(index);
            nodes.Insert(index, buffer);

            //todo：实现更加高效的插入方法，尽可能减少文本复杂度

            //插入标签对
            var parents = nodes.Select(t => t.Parent).ToHashSet();
            foreach (var node in nodes)
            {
                var pair = new PairTagNode(tag, attr);
                node.Parent.InsertChild(node.Parent.Children.IndexOf(node), pair);
                node.Parent.RemoveChild(node);
                pair.AddChild(node);
            }

            //刷新
            foreach (var p in parents)
            {
                p.Refresh();
            }
        }

        /// <summary>
        /// 插入空标签
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tag"></param>
        /// <param name="attr"></param>
        public void InsertEmptyTag(int index, string tag, string attr)
        {
            InnerTextNode node = null;
            foreach (InnerTextNode child in this.Where(t => t is InnerTextNode))
            {
                if (index - child.Length <= 0)
                {
                    node = child;
                    break;
                }
                index -= child.Length;
            }
            if (node != null)
            {
                node.Spilt(index);
                node.Parent.InsertChild(node.Parent.Children.IndexOf(node) + 1, new EmptyTagNode(tag, attr));
            }
            else
            {
                RootNode.AddChild(new EmptyTagNode(tag, attr));
            }
        }

        (List<RichTextNode> nodes, int startIndex, int endIndex) GetNodeBetweenIndex(int startIndex, int endIndex)
        {
            int index = 0;
            int head = 0, end = 0;
            bool flag = true;
            List<RichTextNode> nodes = new List<RichTextNode>();
            foreach (InnerTextNode child in this.Where(t => t is InnerTextNode || t is EmptyTagNode))
            {
                if (child is InnerTextNode)
                {
                    index += child.Length;
                }
                if (index > startIndex)
                {
                    if (flag)
                    {
                        head = child.Length - (index - startIndex);
                        flag = false;
                    }
                    nodes.Add(child);
                }
                if (index >= endIndex)
                {
                    end = child.Length - (index - endIndex);
                    break;
                }
            }
            return (nodes, head, end);
        }

        /// <summary>
        /// 获取纯文本
        /// </summary>
        /// <returns></returns>
        public string GetPlainText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (InnerTextNode elem in this.Where(n => n is InnerTextNode))
            {
                stringBuilder.Append(elem.Text);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 获取源文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return RootNode.ToString();
        }

        public IEnumerator<RichTextNode> GetEnumerator()
        {
            return RootNode.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// 富文本解析节点
    /// </summary>
    public abstract class RichTextNode
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public PairTagNode Parent { get; internal protected set; }

        /// <summary>
        /// 下一个兄弟节点
        /// </summary>
        public RichTextNode NextSibling
        {
            get
            {
                if (Parent != null)
                {
                    int index = Parent.Children.IndexOf(this) + 1;
                    if (index != Parent.Children.Count)
                    {
                        return Parent.Children[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 上一个兄弟节点
        /// </summary>
        public RichTextNode PrevSibling
        {
            get
            {
                if (Parent != null)
                {
                    int index = Parent.Children.IndexOf(this) - 1;
                    if (index != -1)
                    {
                        return Parent.Children[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 后继节点
        /// </summary>
        public RichTextNode Successor => NextSibling ?? Parent.Successor;

        /// <summary>
        /// 前驱节点
        /// </summary>
        public RichTextNode Precursor => PrevSibling ?? Parent;

        /// <summary>
        /// 节点在原文本中的起始位置
        /// </summary>
        /// <remarks>
        /// 该节点第一个字符所在的索引
        /// </remarks>
        public virtual int StartIndex => Parent.EndIndex;

        /// <summary>
        /// 节点在原文本中的终止位置
        /// </summary>
        /// <remarks>
        /// 后继节点第一个字符所在的索引
        /// </remarks>
        public abstract int EndIndex { get; }

        /// <summary>
        /// 文本长度
        /// </summary>
        public virtual int Length => EndIndex - StartIndex;

        public abstract int PlainTextLength { get; }

        /// <summary>
        /// 是否是指定节点的后代节点
        /// </summary>
        /// <remarks>
        /// 当传入自身时返回true
        /// </remarks>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsSubOf(PairTagNode node)
        {
            var p = this;
            while (p != null)
            {
                if (p == node)
                {
                    return true;
                }
                p = p.Parent;
            }
            return false;
        }

        /// <summary>
        /// 矫正内部格式
        /// </summary>
        public virtual void Refresh()
        {
        }
    }

    /// <summary>
    /// 标签节点
    /// </summary>
    public abstract class TagNode : RichTextNode
    {
        /// <summary>
        /// 标签属性值
        /// </summary>
        public string Attr;

        /// <summary>
        /// 节点类型
        /// </summary>
        public readonly string Tag;

        public TagNode(string tag, string attr)
        {
            Tag = tag;
            Attr = attr;
        }
    }

    /// <summary>
    /// 成对标签节点
    /// </summary>
    public class PairTagNode : TagNode, IEnumerable<RichTextNode>
    {
        /// <summary>
        /// 内部元素起始索引
        /// </summary>
        /// <remarks>
        /// 内部元素第一个字符所在的位置
        /// </remarks>
        public virtual int InnerStartIndex => StartIndex + 2 + Attr.Length; //2为<>的长度

        /// <summary>
        /// 内部元素终止索引
        /// </summary>
        /// <remarks>
        /// 闭标签第一个元素所在位置
        /// </remarks>
        public virtual int InnerEndIndex => _children.Last()?.EndIndex ?? InnerStartIndex;//如果不存在子节点，则为内部元素起始索引

        public override int EndIndex => InnerEndIndex + 3 + Tag.Length;//3为</>的长度

        public override int PlainTextLength
        {
            get
            {
                int length = 0;
                foreach (var child in _children)
                {
                    length += child.PlainTextLength;
                }
                return length;
            }
        }

        /// <summary>
        /// 子节点
        /// </summary>
        protected List<RichTextNode> _children = new List<RichTextNode>();

        /// <summary>
        /// <inheritdoc cref="_children"/>
        /// </summary>
        public ReadOnlyCollection<RichTextNode> Children => _children.AsReadOnly();

        public PairTagNode(string tag, string attr) : base(tag, attr) { }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(RichTextNode node)
        {
            _children.Add(node);
            node.Parent = this;
        }

        /// <summary>
        /// 插入子节点
        /// </summary>
        /// <param name="index"></param>
        /// <param name="node"></param>
        public void InsertChild(int index, RichTextNode node)
        {
            _children.Insert(index, node);
            node.Parent = this;
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChild(RichTextNode node)
        {
            _children.Remove(node);
            node.Parent = null;
            if (_children.Count == 0)
            {
                Parent?.RemoveChild(this);
            }
        }

        public override void Refresh()
        {
            //让子节点先刷新
            for (int i = _children.Count - 1; i >= 0; --i)
            {
                _children[i].Refresh();
            }

            //整合直接子节点标签
            for (int i = 0; i < _children.Count; ++i)
            {
                var head = _children[i] as PairTagNode;
                if (head != null)
                {
                    //将后续所有相等的标签对整合入第一个标签
                    while (i + 1 < _children.Count)
                    {
                        var elem = _children[i + 1] as PairTagNode;
                        if (elem?.IsEqualTag(head) ?? false)
                        {
                            RemoveChild(elem);
                            foreach (var child in elem.Children)
                            {
                                head.AddChild(child);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (_children.Count == 0)
            {
                Parent?.RemoveChild(this);
            }

            //合并与自身相同的标签
            PairTagNode p = this;
            while (p._children.Count == 1)
            {
                var next = p._children[0] as PairTagNode;
                if (next == null)
                {
                    break;
                }
                if (next.Tag == Tag)
                {
                    foreach (var child in next.Children)
                    {
                        p.AddChild(child);
                    }
                    p.RemoveChild(next);
                    Attr = next.Attr;
                    next = p;
                }
                p = next;
            }
        }

        /// <summary>
        /// 判断是否是数据相同的标签
        /// </summary>
        /// <param name="pairTag"></param>
        /// <returns></returns>
        public bool IsEqualTag(PairTagNode pairTag)
        {
            //进行标签对比较
            if (pairTag.Tag == Tag)
            {
                return pairTag.Attr == Attr || RichTextUtility.GetPropertys(Attr).ToHashSet().SetEquals(RichTextUtility.GetPropertys(pairTag.Attr));
            }
            return false;
        }

        /// <summary>
        /// 获取内部元素文本
        /// </summary>
        /// <returns></returns>
        public string GetInnerRawText()
        {
            var stringBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                stringBuilder.Append(child.ToString());
            }
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return $"<{Attr}>{GetInnerRawText()}</{Tag}>";
        }

        public IEnumerator<RichTextNode> GetEnumerator()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                yield return _children[i];
                var pair = _children[i] as PairTagNode;
                if (pair != null)
                {
                    foreach (var child in pair)
                    {
                        yield return child;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// 空标签节点
    /// </summary>
    public class EmptyTagNode : TagNode
    {
        public override int EndIndex => StartIndex + 2 + Attr.Length;

        public override int Length => 2 + Attr.Length;

        public override int PlainTextLength => 0;

        public EmptyTagNode(string tag, string value) : base(tag, value) { }

        public override string ToString()
        {
            return $"<{Attr}>";
        }
    }

    /// <summary>
    /// 根标签节点
    /// </summary>
    public class RootNode : PairTagNode
    {
        public override int StartIndex => 0;
        public override int EndIndex => InnerEndIndex;

        public override int InnerStartIndex => StartIndex;

        public RootNode() : base("body", null) { }
        public override string ToString()
        {
            return GetInnerRawText();
        }
    }

    /// <summary>
    /// 内部文字节点
    /// </summary>
    public class InnerTextNode : RichTextNode
    {
        public string Text { get; protected set; }
        public override int EndIndex => StartIndex + Text.Length;

        public override int Length => Text.Length;

        override public int PlainTextLength => Text.Length;

        public InnerTextNode(string text)
        {
            Text = text;
        }

        /// <summary>
        /// 插入字符串
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="value"></param>
        public void Insert(int startIndex, string value)
        {
            Text = Text.Insert(startIndex, value);
        }

        /// <summary>
        /// 移除字符串
        /// </summary>
        /// <remarks>
        /// 当节点的文本内容为空时，会将自身从父节点移除
        /// </remarks>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Remove(int startIndex, int length)
        {
            Text = Text.Remove(startIndex, length);
            if (string.IsNullOrEmpty(Text))
            {
                Parent?.RemoveChild(this);
            }
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <remarks>
        /// 当分割点位于文本开头或结尾时，不做处理并返回自身
        /// </remarks>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public InnerTextNode Spilt(int startIndex)
        {
            if (startIndex == 0 || startIndex == Text.Length)
            {
                return this;
            }
            var node = new InnerTextNode(Text.Substring(startIndex));
            Parent.InsertChild(Parent.Children.IndexOf(this) + 1, node);
            Remove(startIndex, Length - startIndex);
            return node;
        }

        public override void Refresh()
        {
            if (string.IsNullOrEmpty(Text))
            {
                Parent?.RemoveChild(this);
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class RichTextTreeException : Exception
    {
        public RichTextNode Node;
        public RichTextTreeException(RichTextNode node, string message) : base(message) { }
    }
}