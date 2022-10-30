using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System.Text.RegularExpressions;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Text/Alpha", true)]
    public class AlphaTagView : TagView
    {
        public override string Tag => "alpha";

        SliderInt field;

        public AlphaTagView()
        {
            field = new SliderInt(0, 255);
            field.label = "Alpha";
            field.RegisterValueChangedCallback(e => SendChangedEvent());
            field.value = 1;
            Add(field);
        }

        public override string ConvertToText()
        {
            return $"alpha=#{System.Convert.ToString(field.value, 16).PadLeft(2, '0')}";
        }

        public override bool SetValueFromText(string attr)
        {
            if (attr.StartsWith("alpha=#") && Regex.IsMatch(attr.Substring(7), @"^\w\w$"))
            {
                field.value = System.Convert.ToInt32(attr.Substring(7), 16);
                return true;
            }
            return false;
        }
    }
}
