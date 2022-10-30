using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using Sirenix.Serialization;
using System.Linq;

namespace GameToolKit.Behavior.Tree
{
    using NodeStatus = ProcessNode.NodeStatus;
    /// <summary>
    /// ��Ϊ��
    /// </summary>
    [CreateAssetMenu(fileName = "BTree", menuName = "Behavior/Behavior Tree")]
    public class BehaviorTree : CustomGraph<Node>
    {
        #region �ڰ嶨��
        [Serializable]
        public class TreeBlackboard : IBlackboard
        {
            public class ChangeDomainEvent : IBlackboard.BlackboardEventBase
            {
                public enum Domain
                {
                    Prototype,
                    Local
                }
                public Domain NewDomain;
                public Domain OldDomain;
                public ChangeDomainEvent(Domain newDomain, Domain oldDomain, IBlackboard target, string name) : base(target, name)
                {
                    NewDomain = newDomain;
                    OldDomain = oldDomain;
                }
            }
            [OdinSerialize]
            IBlackboard.CallBackList _callBackList = new IBlackboard.CallBackList();
            /// <summary>
            /// ������
            /// </summary>
            /// <remarks>
            /// ÿһ��������ʵ������ñ�����
            /// </remarks>
            [OdinSerialize]
            Dictionary<string, BlackboardVariable> _local = new Dictionary<string, BlackboardVariable>();
            /// <summary>
            /// ����
            /// </summary>
            /// <remarks>
            /// ������ͬһ����Ϊģ�崴����ʵ������ñ�����
            /// </remarks>
            [OdinSerialize]
            Dictionary<string, BlackboardVariable> _prototype = new Dictionary<string, BlackboardVariable>();

            #region �¼�ע��
            public void RegisterCallback<T>(string name, Action<T> callback) where T : IBlackboard.BlackboardEventBase
            {                    
                if (HasValue(name))
                {

                    _callBackList.RegisterCallback(name, callback);
                }
            }

            public void UnregisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : IBlackboard.BlackboardEventBase
            {
                if (HasValue(name))
                {
                    _callBackList.UnregisterCallback(name, callback);
                }
            }

            #endregion

            #region ������ɾ���
            public void RemoveValue(string name)
            {
                Dictionary<string, BlackboardVariable> target;
                if (_local.ContainsKey(name))
                {
                    target = _local;
                }
                else if (_prototype.ContainsKey(name))
                {
                    target = _prototype;
                }
                else
                {
                    return;
                }
                target[name].ValueChanged -= Variable_ValueChanged;
                target.Remove(name);
                var e = new IBlackboard.ValueRemoveEvent(this, name);
                _callBackList.Invoke(name, e);
                _callBackList.RemoveItem(name);
            }

            public void RenameValue(string oldName, string newName)
            {
                Dictionary<string, BlackboardVariable> target;
                if (_local.ContainsKey(oldName))
                {
                    target = _local;
                }
                else if (_prototype.ContainsKey(oldName))
                {
                    target = _prototype;
                }
                else
                {
                    return;
                }
                {
                    var temp = target[oldName];
                    target.Remove(oldName);
                    target.Add(newName, temp);
                }
                var e = new IBlackboard.NameChangedEvent(this, newName, oldName);
                _callBackList.RenameItem(oldName, newName);
                _callBackList.Invoke(newName, e);
            }

            public BlackboardVariable GetVariable(string name)
            {
                if (_local.ContainsKey(name))
                {
                    return _local[name];
                }
                else if (_prototype.ContainsKey(name))
                {
                    return _prototype[name];
                }
                return null;
            }

            public Dictionary<string, BlackboardVariable> GetLocalVariables()
            {
                return new Dictionary<string, BlackboardVariable>(_local);
            }

            public Dictionary<string, BlackboardVariable> GetPrototypeVariables()
            {
                return new Dictionary<string, BlackboardVariable>(_prototype);
            }

            public T GetValue<T>(string name)
            {
                if (_local.ContainsKey(name))
                {
                    return (T)_local[name].Value;
                }
                else if (_prototype.ContainsKey(name))
                {
                    return (T)_prototype[name].Value;
                }
                return default;
            }

            public bool HasValue(string name)
            {
                return _local.ContainsKey(name) || _prototype.ContainsKey(name);
            }

            public void SetValue(string name, object value)
            {
                Dictionary<string, BlackboardVariable> target;
                if (_local.ContainsKey(name))
                {
                    target = _local;
                }
                else if (_prototype.ContainsKey(name))
                {
                    target = _prototype;
                }
                else
                {
                    return;
                }
                target[name].Value = value;
            }

            /// <summary>
            /// ��ӱ�����ı���
            /// </summary>
            /// <param name="name"></param>
            /// <param name="variable"></param>
            public void AddLocalVariable(string name, BlackboardVariable variable)
            {
                _local.Add(name, variable);
                variable.ValueChanged += Variable_ValueChanged;
            }

            /// <summary>
            /// �������ı���
            /// </summary>
            /// <param name="name"></param>
            /// <param name="variable"></param>
            public void AddPrototypeVariable(string name, BlackboardVariable variable)
            {
                _prototype.Add(name, variable);
                variable.ValueChanged += Variable_ValueChanged;
            }

            /// <summary>
            /// Ĭ������뱾����
            /// </summary>
            /// <param name="name"></param>
            /// <param name="variable"></param>
            /// <exception cref="NotImplementedException"></exception>
            public void AddVariable(string name, BlackboardVariable variable)
            {
                AddLocalVariable(name, variable);
            }
            /// <summary>
            /// ��������������
            /// </summary>
            /// <param name="name"></param>
            public void MoveToPrototype(string name)
            {
                if(_local.TryGetValue(name, out var variable))
                {
                    _prototype[name] = variable;
                    _local.Remove(name);
                    _callBackList.Invoke(name, new ChangeDomainEvent(ChangeDomainEvent.Domain.Prototype, ChangeDomainEvent.Domain.Local, this, name));
                }
            }
            /// <summary>
            /// ���������뱾����
            /// </summary>
            /// <param name="name"></param>
            public void MoveToLocal(string name)
            {
                if (_prototype.TryGetValue(name, out var variable))
                {
                    _local[name] = variable;
                    _prototype.Remove(name);
                    _callBackList.Invoke(name, new ChangeDomainEvent(ChangeDomainEvent.Domain.Local, ChangeDomainEvent.Domain.Prototype, this, name));
                }
            }
            private void Variable_ValueChanged(BlackboardVariable sender, object newValue, object oldValue)
            {
                foreach (var variable in _local)
                {
                    if (variable.Value == sender)
                    {
                        var e = new IBlackboard.ValueChangeEvent(this, variable.Key, newValue, oldValue);
                        _callBackList.Invoke(variable.Key, e);
                        return;
                    }
                }
                foreach (var variable in _prototype)
                {
                    if (variable.Value == sender)
                    {
                        var e = new IBlackboard.ValueChangeEvent(this, variable.Key, newValue, oldValue);
                        _callBackList.Invoke(variable.Key, e);
                        return;
                    }
                }
            }
            #endregion

            public TreeBlackboard Clone()
            {
                var bb = new TreeBlackboard();
                foreach(var item in _local)
                {
                    bb.AddLocalVariable(item.Key, item.Value.Clone());
                }
                bb._prototype = _prototype;
                return bb;
            }
        }
        #endregion

        /// <summary>
        /// ���ڵ�
        /// </summary>
        [SerializeField]
        private RootNode _rootNode;
        public RootNode RootNode => _rootNode;
        /// <summary>
        /// �����ֵ�
        /// </summary>
        [NonSerialized, OdinSerialize]
        [NoSaveDuringPlay]
        public TreeBlackboard Blackboard = new TreeBlackboard();
        /// <summary>
        /// ��ǰ�������ж���
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }

        /// <summary>
        /// ��Ϊ������״̬
        /// </summary>
        public bool IsEnable { get; private set; } = false;

        private void Reset()
        {
            _rootNode = CreateNode(typeof(RootNode)) as RootNode;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public NodeStatus Tick()
        {
            if (_rootNode.Status == NodeStatus.Running ||_rootNode.Status == NodeStatus.None)
            {
                _rootNode.Tick();
            }
            return _rootNode.Status;
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        internal BehaviorTree CreateRunningTree(BehaviorTreeRunner runner)
        {
            var tree = CreateInstance<BehaviorTree>();
            tree.Runner = runner;
            tree.Nodes.Clear();
            //���ƺڰ�
            tree.Blackboard = Blackboard.Clone();
            foreach(var var in runner.Variables)
            {
                tree.Blackboard.RemoveValue(var.Key);
                tree.Blackboard.AddLocalVariable(var.Key, var.Value);
            }
            //����ȫ���ڵ�
            foreach(var node in Nodes)
            {
                var n = node.Clone();
                tree.Nodes.Add(n);
                var r = n as RootNode;
                if (r != null)
                {
                    tree._rootNode = r;
                }
            }
            //���ӽڵ�
            foreach(var node in Nodes)
            {
                var clone = tree.FindNode(node.Guid);
                //��������/����
                foreach(var edge in node.InputEdges)
                {
                    clone.InputEdges.Add(new SourceInfo(tree.FindNode(edge.SourceNode.Guid), clone, edge.SourceField, edge.TargetField));
                }
                foreach (var edge in node.OutputEdges)
                {
                    clone.OutputEdges.Add(new SourceInfo(clone, tree.FindNode(edge.TargetNode.Guid), edge.SourceField, edge.TargetField));
                } 
                //������Ϊ��
                var ori = node as ProcessNode;
                if (ori != null)
                {
                    var cn = clone as ProcessNode;
                    foreach (var child in ori.GetChildren())
                    {
                        cn.AddChild(tree.FindNode(child.Guid) as ProcessNode);
                    }
                }
            }
            //��ʼ���ڵ�
            foreach(var node in tree.Nodes)
            {
                node.Init(tree);
            }
            return tree;
        }

        /// <summary>
        /// ������Ϊ��
        /// </summary>
        internal void Enable()
        {
            if (IsEnable)
            {
                return;
            }
            IsEnable = true;
            foreach (var node in Nodes)
            {
                node.OnEnable();
            }
        }

        /// <summary>
        /// ������Ϊ��
        /// </summary>
        internal void Disable()
        {
            if (!IsEnable)
            {
                return;
            }
            IsEnable = false;
            foreach (var node in Nodes)
            {
                node.OnDiable();
            }
        }

        /// <summary>
        /// �Ƴ��ڵ�
        /// </summary>
        /// <param name="node"></param>
        public override void RemoveNode(Node node)
        {
            base.RemoveNode(node);
            //�Ƴ��븸�ڵ������
            var n = node as ProcessNode;
            if (n != null)
            {
                foreach (var item in Nodes)
                {
                    var p = item as ProcessNode;
                    if (p != null)
                    {
                        var children = p.GetChildren();
                        if (children.Contains(node))
                        {
                            p.RemoveChild(n);
                        }
                    }
                }
            }
        }

        public override Node CreateNode(Type type)
        {
            var node = base.CreateNode(type);
            typeof(Node).GetProperty("BehaviorTree").SetValue(node, this);
            return node;
        }
    }
}
