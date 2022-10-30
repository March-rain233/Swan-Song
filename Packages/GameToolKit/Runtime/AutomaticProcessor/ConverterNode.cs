using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace GameToolKit.EventProcessor
{
    [NodeCategory("Converter")]
    public abstract class ConverterNode : Node
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public abstract Type TargeType { get; }
    }

    /// <summary>
    /// 类型转换节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [NodeName("ConverterNode")]
    public class GenericConverterNode<TTarget, TSource> : ConverterNode
    {
        public override Type TargeType => typeof(TTarget);
        [Port("Result", PortDirection.Output)]
        public TTarget Result;
        [Port("Input", PortDirection.Input)]
        public TSource Input;
        protected override void OnValueUpdate()
        {
            Result = (TTarget)Convert.ChangeType(Input, TargeType);
        }
    }

    /// <summary>
    /// 打包节点
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    [NodeName("CombinerNode")]
    public class GenericCombinerNode<T> : ConverterNode
    {
        override public Type TargeType => typeof(T);
        [Port("Result", PortDirection.Output)]
        public T Result;

        protected override void SetValue(string name, object value)
        {
            var type = TargeType;
            do
            {
                var field = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(Result, value);
                    break;
                }
                type = type.BaseType;
            } while (type != null);
        }

        protected override object GetValue(string name)
        {
            return Result;
        }

        protected override void OnValueUpdate()
        {
        }

        public override List<PortData> GetValidPortDataList()
        {
            var list = base.GetValidPortDataList();
            var type = typeof(T);
            while (type != null)
            {
                var portField = type.GetFields(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    ).Where(field => field.DeclaringType == type);
                foreach (var field in portField)
                {
                    var data = new PortData();
                    data.PreferredType = field.FieldType;
                    data.NickName = field.Name;
                    data.Name = field.Name;
                    data.PortDirection = PortDirection.Input;
                    data.PortTypes = new HashSet<Type>() { data.PreferredType };
                    list.Add(data);
                }
                type = type.BaseType;
            }
            return list;
        }
    }

    /// <summary>
    /// 拆分节点
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    [NodeName("SpliterNode")]
    public class GenericSpliterNode<T> : ConverterNode
    {
        override public Type TargeType => typeof(T);
        [Port("Result", PortDirection.Input)]
        public T Result;

        protected override void SetValue(string name, object value)
        {
            Result = (T)value;
        }

        protected override object GetValue(string name)
        {
            var type = TargeType;
            do
            {
                var field = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(Result);
                }
                type = type.BaseType;
            } while (type != null);
            return default;
        }

        protected override void OnValueUpdate()
        {
        }

        public override List<PortData> GetValidPortDataList()
        {
            var list = base.GetValidPortDataList();
            var type = typeof(T);
            while (type != null)
            {
                var portField = type.GetFields(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    ).Where(field => field.DeclaringType == type);
                foreach (var field in portField)
                {
                    var data = new PortData();
                    data.PreferredType = field.FieldType;
                    data.NickName = field.Name;
                    data.Name = field.Name;
                    data.PortDirection = PortDirection.Output;
                    data.PortTypes = new HashSet<Type>() { data.PreferredType };
                    list.Add(data);
                }
                type = type.BaseType;
            }
            return list;
        }
    }
}
