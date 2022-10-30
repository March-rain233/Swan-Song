using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Layout/Align", false)]
    public class AlignTagView : TagView
    {
        public override string Tag => "align";

        DropdownField field;
        List<string> options = new List<string>() { "center", "left", "right" };

        public AlignTagView()
        {
            field = new DropdownField();
            field.label = "Align";
            field.RegisterValueChangedCallback(e => SendChangedEvent());
            field.choices = options;
            field.value = options[0];
            Add(field);
        }

        public override string ConvertToText()
        {
            return $"align={field.value}";
        }

        public override bool SetValueFromText(string attr)
        {
            if (attr.StartsWith("align=") && options.Any(v=>v == attr.Substring(6)))
            {
                field.value = attr.Substring(6);
                return true;
            }
            return false;
        }
    }
}
