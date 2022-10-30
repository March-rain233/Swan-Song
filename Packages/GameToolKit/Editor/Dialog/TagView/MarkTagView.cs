using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Text/Mark", false)]
    public class MarkTagView : TagView
    {
        public override string Tag => "mark";

        ColorField colorField;

        public MarkTagView()
        {
            colorField = new ColorField();
            colorField.label = "Mark Color";
            colorField.value = Color.yellow;
            colorField.RegisterValueChangedCallback(e => SendChangedEvent());
            Add(colorField);
        }

        public override string ConvertToText()
        {
            return $"mark=#{ColorUtility.ToHtmlStringRGBA(colorField.value)}";
        }

        public override bool SetValueFromText(string attr)
        {
            if (attr.StartsWith("mark=") && ColorUtility.TryParseHtmlString(attr.Substring(5), out var color))
            {
                colorField.value = color;
                return true;
            }
            return false;
        }
    }
}
