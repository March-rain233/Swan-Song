using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using GameToolKit.Utility;

namespace GameToolKit.Dialog.Editor
{
    [Tag("Text/Font", false)]
    public class FontTagView : TagView
    {
        public override string Tag => "font";

        TextField font;
        TextField materia;

        public FontTagView()
        {
            font = new TextField();
            font.label = "Font";
            font.RegisterValueChangedCallback(e => SendChangedEvent());
            font.value = "LiberationSans SDF";
            materia = new TextField();
            materia.label = "Materia";
            materia.RegisterValueChangedCallback(e => SendChangedEvent());
            materia.value = string.Empty;
            Add(font);
            Add(materia);
        }

        public override string ConvertToText()
        {
            return $"font=\"{font.value}\"" + (string.IsNullOrEmpty(materia.value) ? "" : $" materia=\"{materia.value}\"");
        }

        public override bool SetValueFromText(string attr)
        {
            if (attr.StartsWith("font="))
            {
                var list = RichTextUtility.GetPropertys(attr);
                font.value = list["font"].Trim('"');
                if(list.TryGetValue("materia", out var value))
                {
                    materia.value = value.Trim('\"');
                }
                return true;
            }
            return false;
        }
    }
}
