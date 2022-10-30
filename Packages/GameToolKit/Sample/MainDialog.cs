using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog 
{
    public class MainDialog : DialogPanelBase
    {
        /// <summary>
        /// ÎÄ±¾¿ØÖÆÆ÷
        /// </summary>
        public TextMeshProUGUI TextController;

        public TextEffectProcessor Processor;

        protected override void Reset()
        {
            base.Reset();
            TextController = TextController ?? GetComponent<TextMeshProUGUI>();
            Processor = new TextEffectProcessor();
        }

        [Button]
        public override void PlayDialog(TextArgument argument, Action onDialogEnd = null)
        {
            var task = Processor.RunTask(TextController, argument.Text);
            task.OnComplete(() =>
            {
                task.EndTask();
                onDialogEnd?.Invoke();
            });
        }

        protected override void OnClose()
        {
            enabled = false;
            Dispose();
        }

        protected override void OnDispose()
        {
            
        }

        protected override void OnHide()
        {
            enabled = false;
        }

        protected override void OnOpen()
        {
            enabled = true;
        }

        protected override void OnShow()
        {
            enabled = true;
        }
    }
}
