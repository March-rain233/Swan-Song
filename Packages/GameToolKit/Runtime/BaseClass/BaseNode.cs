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
        #region �༭����س�Ա
#if UNITY_EDITOR
        /// <summary>
        /// ����
        /// </summary>
        [HideInGraphInspector]
        public string Name;
        /// <summary>
        /// ��ͼλ��
        /// </summary>
        [HideInGraphInspector]
        public Vector2 ViewPosition;
#endif
        #endregion

        #region �ֶ�������
        /// <summary>
        /// Ψһ��ʶ��
        /// </summary>
        [HideInGraphInspector, OdinSerialize]
        public string Guid { get; internal protected set; }

        /// <summary>
        /// �������Ϣ
        /// </summary>
        [SerializeField]
        [HideInGraphInspector]
        public List<SourceInfo> InputEdges = new List<SourceInfo>();

        /// <summary>
        /// �������Ϣ
        /// </summary>
        [SerializeField]
        [HideInGraphInspector]
        public List<SourceInfo> OutputEdges = new List<SourceInfo>();

        /// <summary>
        /// ��һ�����ݸ���ʱ��
        /// </summary>
        [ReadOnly]
        [OdinSerialize]
        public float LastDataUpdataTime { get; protected set; } = 0;
        #endregion

        #region ���ݴ�����ط���

        /// <summary>
        /// ����ָ����Ա��ֵ
        /// </summary>
        /// <remarks>
        /// Ĭ��ͨ������ʵ�֣�������Ż�����������
        /// </remarks>
        /// <param name="name">��Ա����</param>
        /// <param name="value">ֵ</param>
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
        /// ��ȡָ����Ա��ֵ
        /// </summary>
        /// <remarks>
        /// Ĭ��ͨ������ʵ�֣�������Ż�����������
        /// </remarks>
        /// <param name="name">��Ա����</param>
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
        /// ��ȡָ���ֶε�ֵ
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
        /// ����ָ���ֶε�ֵ
        /// </summary>
        protected virtual void PushValue(string fieldName, object value)
        {
            SetValue(fieldName, value);
            OnValueUpdate();
            LastDataUpdataTime = Time.time;
            InitOutputData();
        }

        /// <summary>
        /// ִ�����ݸ����߼�
        /// </summary>
        protected abstract void OnValueUpdate();

        /// <summary>
        /// ��ȡ���������
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
        /// �������������
        /// </summary>
        protected virtual void InitOutputData()
        {
            foreach (var edge in OutputEdges)
            {
                edge.TargetNode.PushValue(edge.TargetField, GetValue(edge.SourceField));
            }
        }

        /// <summary>
        /// �޸��˿�����
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <returns>�¶˿�������������nullʱ��ʾ�޷��޸�����ɾ��������������</returns>
        /// <remarks>
        /// ���ڵ��˿ڱ�������ʱ�����ݴ���
        /// </remarks>
        public virtual string FixPortIndex(string oldIndex)
        {
            Debug.LogWarning($"�ڵ�:{Name} �˿�:{oldIndex} ��ʧ");
            return null;
        }

        /// <summary>
        /// ��ȡ�Ϸ��Ķ˿��б�
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
            /// �ֶε�����
            /// </summary>
            public string Name;
            /// <summary>
            /// �ֶε���ʾ����
            /// </summary>
            public string NickName;
            /// <summary>
            /// �˿�������������
            /// </summary>
            public PortDirection PortDirection;
            /// <summary>
            /// �˿ڿ�ƥ�����������
            /// </summary>
            public HashSet<Type> PortTypes;
            /// <summary>
            /// �˿ڵ���ѡ��������
            /// </summary>
            public Type PreferredType;
        }
    }
}
