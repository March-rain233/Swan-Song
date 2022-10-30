using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace GameToolKit.Utility
{
    /// <summary>
    /// 类型辅助工具
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
        /// 获取类型的所有字段
        /// </summary>
        /// <remarks>
        /// 包括基类的所有符合条件的字段
        /// </remarks>
        /// <param name="type">类型</param>
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
        /// 获取第一个符合名称的字段
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
