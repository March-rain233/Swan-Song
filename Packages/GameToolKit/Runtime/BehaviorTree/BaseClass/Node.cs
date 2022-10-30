using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace GameToolKit.Behavior.Tree
{
    [Serializable]
    [Node]
    [NodeColor("#E4007F")]
    [NodeCategory("NULL")]
    public abstract class Node : BaseNode
    {
        #region 编辑器相关成员
#if UNITY_EDITOR
        /// <summary>
        /// 当节点边框颜色改变时
        /// </summary>
        public event Action<Color> OnColorChanged;
        /// <summary>
        /// 更改节点边框颜色
        /// </summary>
        public void ChangeColor(Color color)
        {
            OnColorChanged?.Invoke(color);
        }
#endif
        #endregion

        #region 字段与属性
        /// <summary>
        /// 节点绑定的行为树
        /// </summary>
        [SerializeField, HideInInspector]
        public BehaviorTree BehaviorTree { get; private set; }
        #endregion

        #region 生命周期相关
        /// <summary>
        /// 克隆当前节点
        /// </summary>
        /// <remarks>
        /// 默认为浅拷贝，需要深拷贝时请进行重载
        /// </remarks>
        public virtual Node Clone()
        {
            var node = MemberwiseClone() as Node;
            node.Guid = Guid;
            node.InputEdges = new List<SourceInfo>();
            node.OutputEdges = new List<SourceInfo>();
            node.LastDataUpdataTime = 0;
            return node;
        }

        /// <summary>
        /// 初始化节点
        /// </summary>
        /// <param name="tree"></param>
        internal void Init(BehaviorTree tree)
        {
            BehaviorTree = tree;
            OnInit();
        }

        /// <summary>
        /// 当节点初始化时
        /// </summary>
        /// <param name="tree"></param>
        protected virtual void OnInit() { }

        /// <summary>
        /// 当行为树启用时
        /// </summary>
        internal protected virtual void OnEnable() { }

        /// <summary>
        /// 当行为树禁用时
        /// </summary>
        internal protected virtual void OnDiable() { }
        #endregion
    }
}
