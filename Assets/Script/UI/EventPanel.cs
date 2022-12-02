using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using GameToolKit.Dialog;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class EventPanel : DialogPanelBase, IOptionalView
{
    public class EventTextArgument: TextArgument
    {
        public Sprite EventImage;
        public string EventName;
    }

    public TextMeshProUGUI TxtName;
    public TextMeshProUGUI TxtBody;
    public Image EventImage;
    public VerticalLayoutGroup Options;
    public GameObject OptionModel;

    public override void PlayDialog(TextArgument argument, Action onDialogEnd = null)
    {
        var arg = argument as EventTextArgument;
        TxtName.text = arg.EventName;
        TxtBody.text = arg.Text;
        EventImage.sprite = arg.EventImage;

        onDialogEnd?.Invoke();
    }

    public void ShowOptions(List<OptionArgument> options, Action<int> onSelected)
    {
        for(int i = Options.transform.childCount - 1; i >= 0; --i)
        {
            Destroy(Options.transform.GetChild(i).gameObject);
        }
        for(int i = 0; i < options.Count; ++i)
        {
            int index = i;
            var obj = Instantiate(OptionModel, Options.transform);
            obj.SetActive(true);
            var button = obj.GetComponent<Button>();
            var text = obj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = options[i].Option;
            button.onClick.AddListener(() =>
            {
                onSelected(index);
            });

            button.interactable = options[i].IsEnable;
        }
    }

    protected override void OnClose()
    {
        base.OnClose();
    }

    protected override void OnWaitingListEmpty()
    {
        GameManager.Instance.SetStatus<SelectLevelState>();
    }
}
