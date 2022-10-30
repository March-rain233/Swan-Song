using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System.Text.RegularExpressions;
using GameToolKit.Editor;

namespace GameToolKit.Dialog.Editor
{
    /// <summary>
    /// 携带单位用于表述长度的标签基类
    /// </summary>
    public abstract class LengthTagView : TagView
    {
        FieldWithUnit field;
        protected virtual List<string> choices => new List<string>() { "init", "em", "%" };

        protected abstract string _fieldName { get; }

        string units
        {
            get
            {
                var t = "";
                foreach(var c in choices)
                {
                    t += $"|{c}";
                }
                return t.TrimEnd('|');
            }
        }

        public LengthTagView()
        {
            field = new FieldWithUnit();
            field.Label = _fieldName;
            field.Field.RegisterValueChangedCallback(e => SendChangedEvent());
            field.Unit.RegisterValueChangedCallback(e => SendChangedEvent());
            field.Value = 0;
            field.UnitValue = choices[0];
            field.Choice = choices;
            Add(field);
        }

        public override string ConvertToText()
        {
            return $"{Tag}={field.Value}{(field.UnitValue == "init" ? "" : field.UnitValue)}";
        }

        public override bool SetValueFromText(string attr)
        {
            if (attr.StartsWith($"{Tag}=") && Regex.IsMatch(attr.Substring(Tag.Length + 1), $@"^(-?\d+)(\.\d+)?({units})?$"))
            {
                field.Value = System.Convert.ToSingle(Regex.Match(attr.Substring(Tag.Length + 1), @"^(-?\d+)(\.\d+)?").Value);
                var unit = Regex.Match(attr.Substring(Tag.Length + 1), $"({units})?$").Value;
                field.UnitValue = string.IsNullOrEmpty(unit) ? "init" : unit;
                return true;
            }
            return false;
        }
    }
}
