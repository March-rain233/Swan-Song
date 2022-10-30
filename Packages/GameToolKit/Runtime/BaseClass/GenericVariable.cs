using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameToolKit
{
    public abstract class GenericVariable<T> : BlackboardVariable
    {
        [SerializeField]
        [OnValueChanged("OnValueChangedEditor", true)]
        T _value;

        bool _isReadOnly = false;

        public override bool IsReadOnly => _isReadOnly;

        public override Type TypeOfValue => typeof(T);

        protected override object GetValue()
        {
            return _value;
        }
        protected override void SetValue(object value)
        {
            _value = (T)value;
        }

#if UNITY_EDITOR
        T _last;
        private void OnValueChangedEditor()
        {
            OnValueChanged(_value, _last);
            _last = _value;
        }
#endif
    }
    public class ObjectVariable : GenericVariable<object> { }
    public class StringVariable : GenericVariable<string> { }
    public class BooleanVariable : GenericVariable<bool> { }
    public class DoubleVarialbe : GenericVariable<double> { }
    public class FloatVarialbe : GenericVariable<float> { }
    public class IntVarialbe : GenericVariable<int> { }
    public class Vector2Variable : GenericVariable<Vector2> { }
    public class Vector3Variable : GenericVariable<Vector3> { }
    public class Vector4Variable : GenericVariable<Vector4> { }
    public class ColorVariable : GenericVariable<Color> { }
    public class GameObjectVariable : GenericVariable<GameObject> { }
    public class ScriptableObjectVariable : GenericVariable<ScriptableObject> { }
    public class ComponentVariable : GenericVariable<Component> { }
    public class RectVariable : GenericVariable<Rect> { }
    public class BoundsVariable : GenericVariable<Bounds> { }
    public class CurveVariable : GenericVariable<AnimationCurve> { }
}
