using GameToolKit.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameToolKit {
    public class OptionPanel : OptionPanelBase
    {
        public override void ShowOptions(List<OptionArgument> options, Action<int> onSelected)
        {
            var model = UISetting.Instance.PrefabsDic["Option"];
            for(int i = 0;i< options.Count; ++i)
            {
                int index = i;
                var view = Instantiate(model, transform).GetComponent<Button>();
                view.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = options[i].Option;
                view.onClick.AddListener(() =>
                {
                    onSelected(index);
                    ServiceFactory.Instance.GetService<PanelManager>().ClosePanel(this);
                });
            }
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

        protected override void OnInit()
        {
            
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
