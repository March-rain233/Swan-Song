using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// ������������
    /// </summary>
    public enum LifeType
    {
        /// <summary>
        /// ������
        /// </summary>
        Presistent,
        /// <summary>
        /// �������ı������
        /// </summary>
        WhenSceneChanged,
    }

    /// <summary>
    /// �ڰ��������
    /// </summary>
    [System.Serializable]
    public abstract class BlackboardVariable
    {
        [System.Serializable]
        public delegate void VariableChangedHandler(BlackboardVariable sender, object newValue, object oldValue);

        /// <summary>
        /// ��������
        /// </summary>
        public LifeType LifeType;

        /// <summary>
        /// ����ֵ
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
        /// ������ֵ�仯ʱ�����¼�
        /// </summary>
        [SerializeField]
        public event VariableChangedHandler ValueChanged;

        /// <summary>
        /// ��¡����
        /// </summary>
        /// <remarks>
        /// Ĭ��Ϊǳ�������¼��Ķ����б����и���
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
        /// ���ñ���ֵ
        /// </summary>
        /// <remarks>
        /// �����������ظú�������ȷ���ñ���
        /// </remarks>
        /// <param name="value"></param>
        protected virtual void SetValue(object value) { }

        /// <summary>
        /// ��ȡ����ֵ
        /// </summary>
        /// <remarks>
        /// �����������ظú��������ر���ֵ
        /// </remarks>
        /// <returns></returns>
        protected abstract object GetValue();
    }
}
