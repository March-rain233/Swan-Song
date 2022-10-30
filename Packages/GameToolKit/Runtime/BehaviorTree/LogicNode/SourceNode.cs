using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameToolKit.Behavior.Tree
{
    [NodeCategory("Logic/Input")]
    public abstract class SourceNode : LogicNode
    {
        protected override void OnInit()
        {
            base.OnInit();
            InitOutputData();
        }
    }
}