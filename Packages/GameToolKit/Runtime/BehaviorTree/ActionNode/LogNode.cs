using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace GameToolKit.Behavior.Tree
{
    public class LogNode : ActionNode
    {
        [Port("Message", PortDirection.Input)]
        [TextArea]
        public string Message;

        public InfoMessageType MessageType;
        protected override NodeStatus OnUpdate()
        {
            switch (MessageType)
            {
                case InfoMessageType.None:
                case InfoMessageType.Info:
                    Debug.Log(Message);
                    break;
                case InfoMessageType.Warning:
                    Debug.LogWarning(Message);
                    break;
                case InfoMessageType.Error:
                    Debug.LogError(Message);
                    break;
                default:
                    break;
            }
            return NodeStatus.Success;
        }
    }
}
