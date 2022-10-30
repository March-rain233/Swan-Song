using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace GameToolKit.Utility
{
    /// <summary>
    /// ���͸�������
    /// </summary>
    public static class TypeUtility
    {
        class TypeComparer : IEqualityComparer<FieldInfo>
        {
            public bool Equals(FieldInfo x, FieldInfo y)
            {
                return x.Name == y.Name && x.DeclaringType == y.DeclaringType;
            }

            public int GetHashCode(FieldInfo obj)
            {
                return $"{obj.Name}+{obj.DeclaringType}".GetHashCode();
            }
        }

        /// <summary>
        /// ��ȡ���͵������ֶ�
        /// </summary>
        /// <remarks>
        /// ������������з����������ֶ�
        /// </remarks>
        /// <param name="type">����</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetAllField(Type type, BindingFlags flags = BindingFlags.Public| BindingFlags.NonPublic| BindingFlags.Instance)
        {
            return GetAllField(type, typeof(object), flags);
        }
       
        public static IEnumerable<FieldInfo> GetAllField(Type type, Type endType, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var list = new HashSet<FieldInfo>(new TypeComparer());
            if (!type.IsSubclassOf(endType))
            {
                throw new ArgumentException($"{type} is not Subclass of {endType}");
            }
            while(type != endType.BaseType)
            {
                list.UnionWith(type.GetFields(flags));
                type = type.BaseType;
            }
            var test = list.Where(e=>e.Name == "_value").ToArray();
            return list;
        }

        /// <summary>
        /// ��ȡ��һ���������Ƶ��ֶ�
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static FieldInfo GetField(string name, Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            return GetField(name, type, typeof(object), flags);
        }

        public static FieldInfo GetField(string name, Type type, Type endType, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (!type.IsSubclassOf(endType))
            {
                throw new ArgumentException($"{type} is not Subclass of {endType}");
            }
            while (type != endType.BaseType)
            {
                var field = type.GetField(name, flags);
                if (field != null)
                {
                    return field;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
