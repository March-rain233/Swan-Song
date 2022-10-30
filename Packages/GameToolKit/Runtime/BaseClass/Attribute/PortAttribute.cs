using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// 定义显示端口的类型
    /// </summary>
    /// <remarks>
    /// 如果不填写extendporttypes参数，则默认为变量的类型
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class PortAttribute : Attribute
    {
        string _name;
        PortDirection _direction;
        Type[] _extendPortTypes;
        bool _isMemberFields;

        public string Name => _name;
        public PortDirection Direction => _direction; 
        public Type[] ExtendPortTypes => _extendPortTypes;

        public bool IsMemberFields => _isMemberFields;

        /// <summary>
        /// 端口类型
        /// </summary>
        /// <param name="name">端口显示的名字</param>
        /// <param name="direction">端口输入输出方向</param>
        /// <param name="extendPortTypes">额外的端口可匹配的数据类型</param>
        public PortAttribute(string name, PortDirection direction, Type[] extendPortTypes = null)
        {
            _name = name;
            _direction = direction;
            _extendPortTypes = extendPortTypes;
            _isMemberFields = false;
        }
        /// <summary>
        /// 端口类型
        /// </summary>
        /// <param name="name">端口显示的名字</param>
        /// <param name="direction">端口输入输出方向</param>
        /// <param name="isMemberFields">是否是暴露内部字段而不是对象本身</param>
        public PortAttribute(string name, PortDirection direction, bool isMemberFields)
        {
            _name = name;
            _direction = direction;
            _extendPortTypes = null;
            _isMemberFields = isMemberFields;
        }
    }
    public enum PortDirection
    {
        Input,
        Output,
    }
}