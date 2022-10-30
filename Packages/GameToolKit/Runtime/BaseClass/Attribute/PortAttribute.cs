using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// ������ʾ�˿ڵ�����
    /// </summary>
    /// <remarks>
    /// �������дextendporttypes��������Ĭ��Ϊ����������
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
        /// �˿�����
        /// </summary>
        /// <param name="name">�˿���ʾ������</param>
        /// <param name="direction">�˿������������</param>
        /// <param name="extendPortTypes">����Ķ˿ڿ�ƥ�����������</param>
        public PortAttribute(string name, PortDirection direction, Type[] extendPortTypes = null)
        {
            _name = name;
            _direction = direction;
            _extendPortTypes = extendPortTypes;
            _isMemberFields = false;
        }
        /// <summary>
        /// �˿�����
        /// </summary>
        /// <param name="name">�˿���ʾ������</param>
        /// <param name="direction">�˿������������</param>
        /// <param name="isMemberFields">�Ƿ��Ǳ�¶�ڲ��ֶζ����Ƕ�����</param>
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