using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 过程调用异常
    /// </summary>
    [SerializeField]
    public class ProcessException : System.ApplicationException
    {
        public ProcessNode Node;
        public ProcessException(ProcessNode node, string msg):base(msg)
        {
            Node = node;
        }
    }
}
