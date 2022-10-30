using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 行为树运行脚本
    /// </summary>
    [AddComponentMenu("AI/BehaviorTreeRunner")]
    public class BehaviorTreeRunner : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public BehaviorTree ModelTree
        {
            get => _modelTree;
            set
            {
                SetModelTree(value);
            }
        }
        [SerializeField]
        private BehaviorTree _modelTree;

        public Dictionary<string, BlackboardVariable> Variables = new Dictionary<string, BlackboardVariable>();

        /// <summary>
        /// 当前运行的树
        /// </summary>
        public BehaviorTree RunTree { get; private set; } = null;

        private void Awake()
        {
            RunTree = ModelTree.CreateRunningTree(this);
        }

        private void OnEnable()
        {
            RunTree.Enable();
        }

        private void OnDisable()
        {
            RunTree.Disable();
        }

        private void Update()
        {
            if (RunTree.IsEnable && RunTree.Tick() == ProcessNode.NodeStatus.Success)
            {
                enabled = false;
            }
        }

        private void SetModelTree(BehaviorTree tree)
        {
            if (_modelTree)
            {
                foreach(var variable in Variables)
                {
                    _modelTree.Blackboard.UnregisterCallback<IBlackboard.ValueRemoveEvent>(variable.Key, RemoveItem);
                    _modelTree.Blackboard.UnregisterCallback<IBlackboard.NameChangedEvent>(variable.Key, RenameItem);
                }
            }
            Variables.Clear();
            _modelTree = tree;
        }

        public void RemoveItem(IBlackboard.ValueRemoveEvent e)
        {
            Variables.Remove(e.Name);
        }

        public void RenameItem(IBlackboard.NameChangedEvent e)
        {
            var temp = Variables[e.OldName];
            Variables.Remove(e.OldName);
            Variables[e.Name] = temp;
        }
    }
}
