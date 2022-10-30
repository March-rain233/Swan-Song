using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameToolKit.Utility
{
    public static class RichTextUtility
    {
        static HashSet<string> _nativeTags = new HashSet<string>()
        {
            "align",
            "allcaps",
            "alpha",
            "b",
            "br",
            "color",
            "cspace",
            "font",
            "font-weight",
            "gradient",
            "i",
            "indent",
            "line-height",
            "line-indent",
            "link",
            "lowercase",
            "margin",
            "mark",
            "mspace",
            "nobr",
            "noparse",
            "page",
            "pos",
            "rotate",
            "s",
            "size",
            "smallcaps",
            "space",
            "sprite",
            "strikethrough",
            "style",
            "sub",
            "sup",
            "u",
            "uppercase",
            "voffset",
            "width",
        };

        /// <summary>
        /// Unity原生标签
        /// </summary>
        public static HashSet<string> NativeTags => _nativeTags;

        /// <summary>
        /// 获取纯文本
        /// </summary>
        /// <param name="rawText"></param>
        /// <returns></returns>
        public static string GetPlainText(string rawText)
        {
            return Regex.Replace(rawText, @"<[^<>]*>", "");
        }

        /// <summary>
        /// 从标签的属性内拆解出数据对
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPropertys(string attr)
        {
            var matchs = Regex.Matches(attr, "\\w*?=(\"([^\"]|\\\\\")\"|[^ ]*)");
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (Match match in matchs)
            {
                var temp = match.Value;
                result.Add(Regex.Match(temp, ".*(?==)").Value, Regex.Match(temp, "(?<==).*").Value);
            }
            return result;
        }

        /// <summary>
        /// 获取标签名
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static string GetTagType(string attr)
        {
            return Regex.Match(attr, "^(\\w|-)*").Value;
        }
    }
}
