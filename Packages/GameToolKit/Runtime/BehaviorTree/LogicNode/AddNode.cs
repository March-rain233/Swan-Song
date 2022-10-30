using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree {
    public class AddNode : LogicNode
    {
        [Port("InputA", PortDirection.Input, new System.Type[]{ typeof(int), typeof(float) })]
        public double InputA;
        [Port("InputB", PortDirection.Input, new System.Type[] { typeof(int), typeof(float) })]
        public double InputB;
        [Port("Result", PortDirection.Output, new System.Type[] { typeof(int), typeof(float) })]
        public double Result;
        protected override void OnValueUpdate()
        {
            Result = InputA + InputB;
        }

        protected override object GetValue(string name)
        {
            return Result;
        }
    }
}
