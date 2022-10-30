using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using GameToolKit.Utility;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 文本效果处理器
    /// </summary>
    public class TextEffectProcessor
    {
        /// <summary>
        /// 标签上下文
        /// </summary>
        public class TagContext
        {
            public TextEffectTask Target;
            public TagNode Tag;
        }

        /// <summary>
        /// 标签渲染上下文
        /// </summary>
        public class TagRenderContext : TagContext
        {
            /// <summary>
            /// 文本显示信息
            /// </summary>
            public TMP_TextInfo TextInfo;

            /// <summary>
            /// 当前文字在显示文本中的位置索引
            /// </summary>
            public int Index;

            /// <summary>
            /// 当前文字在标签中所处的位置索引
            /// </summary>
            public int IndexInTag;
        }
        public delegate void CharacterRenderHandler(TagRenderContext context);
        public delegate void TagInvokeHandler(TagContext context);
        public delegate void TagCancelHandler(TagContext context);

        /// <summary>
        /// 标签处理器函数集
        /// </summary>
        public struct TagProcessor
        {
            public TagInvokeHandler OnTagInvoke;
            public TagCancelHandler OnTagCancel;
            public CharacterRenderHandler OnRenderCharacter;
        }

        public Dictionary<string, TagProcessor> TagProcessors = new Dictionary<string, TagProcessor>();

        public TextEffectProcessor()
        {
            var speed = new TagProcessor()
            {
                OnTagInvoke = context => context.Target.SpeedStack.Push(Convert.ToSingle(RichTextUtility.GetPropertys(context.Tag.Attr)["speed"])),
                OnTagCancel = context => context.Target.SpeedStack.Pop(),
            };
            TagProcessors["speed"] = speed;

            var comma = new TagProcessor()
            {
                OnTagInvoke = context => context.Target.NextCharacterTime = Convert.ToSingle(RichTextUtility.GetPropertys(context.Tag.Attr)["comma"]),
            };
            TagProcessors["comma"] = comma;
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textMesh"></param>
        /// <returns></returns>
        public TextEffectTask RunTask(TextMeshProUGUI textMesh, string text)
        {
            var task = new TextEffectTask(this, textMesh, text);
            return task;
        }
    }
}
