using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Text/Color", false)]
    public class ColorTagView : TagView
    {
        public override string Tag => "color";

        ColorField colorField;

        public ColorTagView()
        {
            colorField = new ColorField();
            colorField.label = "Text Color";
            colorField.value = Color.black;
            colorField.RegisterValueChangedCallback(e => SendChangedEvent());
            Add(colorField);
        }

        public override string ConvertToText()
        {
            return $"color=#{ColorUtility.ToHtmlStringRGBA(colorField.value)}";
        }

        public override bool SetValueFromText(string attr)
        {
            if(attr.StartsWith("color=") && ColorUtility.TryParseHtmlString(attr.Substring(6), out var color))
            {
                colorField.value = color;
                return true;
            }
            return false;
        }
    }
}
