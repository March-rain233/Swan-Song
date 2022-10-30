using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// 生命周期类型
    /// </summary>
    public enum LifeType
    {
        /// <summary>
        /// 不销毁
        /// </summary>
        Presistent,
        /// <summary>
        /// 当场景改变后销毁
        /// </summary>
        WhenSceneChanged,
    }

    /// <summary>
    /// 黑板变量基类
    /// </summary>
    [System.Serializable]
    public abstract class BlackboardVariable
    {
        [System.Serializable]
        public delegate void VariableChangedHandler(BlackboardVariable sender, object newValue, object oldValue);

        /// <summary>
        /// 生命周期
        /// </summary>
        public LifeType LifeType;

        /// <summary>
        /// 变量值
        /// </summary>
        public object Value
        {
            get => GetValue();
            set
            {
                if (IsReadOnly)
                {
                    Debug.LogError("An attempt was made to assign a value to a variable, but it is read-only");
                    return;
                }
                var oldValue = GetValue();
                SetValue(value);
                ValueChanged?.Invoke(this, oldValue, value);
            }
        }

        public abstract bool IsReadOnly { get; }

        public abstract Type TypeOfValue { get; }

        /// <summary>
        /// 当变量值变化时触发事件
        /// </summary>
        [SerializeField]
        public event VariableChangedHandler ValueChanged;

        /// <summary>
        /// 克隆变量
        /// </summary>
        /// <remarks>
        /// 默认为浅拷贝，事件的订阅列表不进行复制
        /// </remarks>
        /// <returns></returns>
        public virtual BlackboardVariable Clone()
        {
            var obj = MemberwiseClone() as BlackboardVariable;
            var list = obj.ValueChanged?.GetInvocationList();
            if (list != null)
            {
                foreach (VariableChangedHandler item in list)
                {
                    obj.ValueChanged -= item;
                }
            }
            return obj;
        }

        protected void OnValueChanged(object newValue, object oldValue)
        {
            ValueChanged?.Invoke(this, newValue, oldValue);
        }

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <remarks>
        /// 派生类请重载该函数来正确设置变量
        /// </remarks>
        /// <param name="value"></param>
        protected virtual void SetValue(object value) { }

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <remarks>
        /// 派生类请重载该函数来返回变量值
        /// </remarks>
        /// <returns></returns>
        protected abstract object GetValue();
    }
}
