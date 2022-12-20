using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using System;
using GameToolKit.Utility;
using System.Reflection;

namespace GameToolKit
{
    public abstract class BaseNode
    {
        #region 编辑器相关成员
#if UNITY_EDITOR
        /// <summary>
        /// 名称
        /// </summary>
        [HideInGraphInspector]
        public string Name;
        /// <summary>
        /// 视图位置
        /// </summary>
        [HideInGraphInspector]
        public Vector2 ViewPosition;
#endif
        #endregion

        #region 字段与属性
        /// <summary>
        /// 唯一标识符
        /// </summary>
        [HideInGraphInspector, OdinSerialize]
        public string Guid { get; internal protected set; }

        /// <summary>
        /// 输入边信息
        /// </summary>
        [SerializeField]
        [HideInGraphInspector]
        public List<SourceInfo> InputEdges = new List<SourceInfo>();

        /// <summary>
        /// 输出边信息
        /// </summary>
        [SerializeField]
        [HideInGraphInspector]
        public List<SourceInfo> OutputEdges = new List<SourceInfo>();

        /// <summary>
        /// 上一次数据更新时间
        /// </summary>
        [ReadOnly]
        [OdinSerialize]
        public float LastDataUpdataTime { get; protected set; } = 0;
        #endregion

        #region 数据传输相关方法

        /// <summary>
        /// 设置指定成员的值
        /// </summary>
        /// <remarks>
        /// 默认通过反射实现，如果想优化性能请重载
        /// </remarks>
        /// <param name="name">成员名称</param>
        /// <param name="value">值</param>
        protected virtual void SetValue(string name, object value)
        {
            var list = name.Split('.');
            FieldInfo field = TypeUtility.GetField(list[0], GetType(), typeof(BaseNode));
            object temp = this;
            for(int i = 1; i < list.Length; i++)
            {
                temp = field.GetValue(temp);
                field = TypeUtility.GetField(list[i], field.FieldType);
            }
            field.SetValue(temp, value);
        }

        /// <summary>
        /// 获取指定成员的值
        /// </summary>
        /// <remarks>
        /// 默认通过反射实现，如果想优化性能请重载
        /// </remarks>
        /// <param name="name">成员名称</param>
        protected virtual object GetValue(string name)
        {
            var list = name.Split('.');
            FieldInfo field = TypeUtility.GetField(list[0], GetType(), typeof(BaseNode));
            object temp = this;
            for (int i = 1; i < list.Length; i++)
            {
                temp = field.GetValue(temp);
                field = TypeUtility.GetField(list[i], field.FieldType);
            }
            return field.GetValue(temp);
        }

        /// <summary>
        /// 拉取指定字段的值
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected virtual object PullValue(string fieldName)
        {
            if (LastDataUpdataTime != Time.time)
            {
                InitInputData();
                OnValueUpdate();
                LastDataUpdataTime = Time.time;
                InitOutputData();
            }
            return GetValue(fieldName);
        }

        /// <summary>
        /// 推送指定字段的值
        /// </summary>
        protected virtual void PushValue(string fieldName, object value)
        {
            SetValue(fieldName, value);
            OnValueUpdate();
            LastDataUpdataTime = Time.time;
            InitOutputData();
        }

        /// <summary>
        /// 执行数据更新逻辑
        /// </summary>
        protected abstract void OnValueUpdate();

        /// <summary>
        /// 拉取输入的数据
        /// </summary>
        protected virtual void InitInputData()
        {
            foreach (var edge in InputEdges)
            {
                var obj = edge.SourceNode.PullValue(edge.SourceField);
                SetValue(edge.TargetField, obj);
            }
        }

        /// <summary>
        /// 推送输出的数据
        /// </summary>
        protected virtual void InitOutputData()
        {
            foreach (var edge in OutputEdges)
            {
                edge.TargetNode.PushValue(edge.TargetField, GetValue(edge.SourceField));
            }
        }

        /// <summary>
        /// 修复端口索引
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <returns>新端口索引；当返回null时表示无法修复，将删除该条连接数据</returns>
        /// <remarks>
        /// 用于当端口变量更名时的数据错误
        /// </remarks>
        public virtual string FixPortIndex(string oldIndex)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"节点:{Name} 端口:{oldIndex} 丢失");
#endif
            return null;
        }

        /// <summary>
        /// 获取合法的端口列表
        /// </summary>
        /// <returns></returns>
        public virtual List<PortData> GetValidPortDataList()
        {
            var list = new List<PortData>();
            var fields = TypeUtility.GetAllField(GetType(), typeof(BaseNode))
                .Where(field => field.IsDefined(typeof(PortAttribute), true));
            foreach (var field in fields)
            {
                var attrs = field.GetCustomAttributes(typeof(PortAttribute), true);
                foreach (PortAttribute attr in attrs)
                {
                    if (attr.IsMemberFields)
                    {
                        var children = TypeUtility.GetAllField(field.FieldType);
                        foreach(var child in children)
                        {
                            var data = new PortData();
                            data.PreferredType = child.FieldType;
                            data.NickName = child.Name;
                            data.Name = $"{field.Name}.{child.Name}";
                            data.PortDirection = attr.Direction;
                            data.PortTypes = new HashSet<Type>() { data.PreferredType };
                            list.Add(data);
                        }
                    }
                    else
                    {
                        var data = new PortData();
                        data.PreferredType = field.FieldType;
                        data.NickName = attr.Name;
                        data.Name = field.Name;
                        data.PortDirection = attr.Direction;
                        data.PortTypes = new HashSet<Type>() { data.PreferredType };
                        if (attr.ExtendPortTypes != null)
                        {
                            data.PortTypes.UnionWith(attr.ExtendPortTypes);
                        }
                        list.Add(data);
                    }
                }
            }
            return list;
        }
        #endregion
        public struct PortData
        {
            /// <summary>
            /// 字段的名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 字段的显示名称
            /// </summary>
            public string NickName;
            /// <summary>
            /// 端口数据流动方向
            /// </summary>
            public PortDirection PortDirection;
            /// <summary>
            /// 端口可匹配的数据类型
            /// </summary>
            public HashSet<Type> PortTypes;
            /// <summary>
            /// 端口的首选数据类型
            /// </summary>
            public Type PreferredType;
        }
    }
}
