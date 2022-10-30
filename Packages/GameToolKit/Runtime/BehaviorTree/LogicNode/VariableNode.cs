using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
namespace GameToolKit.Behavior.Tree
{
    public class VariableNode<T> : InputNode<T>
    {
        [OdinSerialize]
        [HideInInspector]
        private string _index = "";

        [OdinSerialize]
        [ValueDropdown("GetValidIndex", AppendNextDrawer = true)]
        [InfoBox("The index is not contained in the dataset", InfoMessageType.Warning, "IsNotContainIndex")]
        [DelayedProperty]
        public string Index
        {
            get { return _index; }
            set
            {
                var old = _index;
                if (!string.IsNullOrEmpty(_index))
                {
                    _blackboard?.UnregisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
                    _blackboard?.UnregisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
                }
                _index = value;
                if(_index != null && old != null)//���old����null��˵���������ڱ����л�����Ӧ���ظ���
                {
                    _blackboard?.RegisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
                    _blackboard?.RegisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
                    if (_blackboard != null && _blackboard.HasValue(_index))
                    {
                        Value = _blackboard.GetValue<T>(_index);
                    }
                }
            }
        }

        /// <summary>
        /// �Ƿ񲻴��ڸ�����
        /// </summary>
        public bool IsNotContainIndex => !GetValidIndex().Contains(Index); 

        protected virtual IBlackboard _blackboard => BehaviorTree.Blackboard;

        protected override void OnValueUpdate()
        {
            if (_blackboard != null && _blackboard.HasValue(_index))
            {
                _value = _blackboard.GetValue<T>(_index);
            }
        }

        protected override void OnInit()
        {
            _value = _blackboard.GetValue<T>(_index);
            _blackboard.RegisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
            _blackboard.RegisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
            //todo:���������Ƴ�ʱ���߼�
            base.OnInit();
        }

        protected void OnIndexChanged(IBlackboard.NameChangedEvent e)
        {
            _index = e.Name;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(BehaviorTree);
#endif
        }

        protected void OnValueChanged(IBlackboard.ValueChangeEvent e)
        {
            Value = (T)e.NewValue;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(BehaviorTree);
#endif
        }

        /// <summary>
        /// ��ȡ���ڵ������б�
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetValidIndex()
        {
            var blackboard = _blackboard as BehaviorTree.TreeBlackboard;
            List<string> validIndex = new List<string>();
            var type = typeof(T);
            foreach (var blackboardItem in blackboard.GetLocalVariables())
            {
                if(blackboardItem.Value.TypeOfValue == type)
                {
                    validIndex.Add(blackboardItem.Key);
                }

            }
            foreach (var blackboardItem in blackboard.GetPrototypeVariables())
            {
                if (blackboardItem.Value.TypeOfValue == type)
                {
                    validIndex.Add(blackboardItem.Key);
                }
            }
            return validIndex;
        }
    }
}
