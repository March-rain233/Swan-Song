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
    /// �ı�Ч��������
    /// </summary>
    public class TextEffectProcessor
    {
        /// <summary>
        /// ��ǩ������
        /// </summary>
        public class TagContext
        {
            public TextEffectTask Target;
            public TagNode Tag;
        }

        /// <summary>
        /// ��ǩ��Ⱦ������
        /// </summary>
        public class TagRenderContext : TagContext
        {
            /// <summary>
            /// �ı���ʾ��Ϣ
            /// </summary>
            public TMP_TextInfo TextInfo;

            /// <summary>
            /// ��ǰ��������ʾ�ı��е�λ������
            /// </summary>
            public int Index;

            /// <summary>
            /// ��ǰ�����ڱ�ǩ��������λ������
            /// </summary>
            public int IndexInTag;
        }
        public delegate void CharacterRenderHandler(TagRenderContext context);
        public delegate void TagInvokeHandler(TagContext context);
        public delegate void TagCancelHandler(TagContext context);

        /// <summary>
        /// ��ǩ������������
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
        /// ��������
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
