using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using UnityEngine.UI;
using System;
using TMPro;

public class ArtifactSelectPanel : PanelBase
{
    public Transform Root;
    public ArcLayout ArcLayout;
    public TextMeshProUGUI TxtTitle;
    public Button BtnBack;

    public GameObject ViewModel;

    public event Action<Artifact> Selected;
    public event Action Quitting;
    protected override void OnInit()
    {
        base.OnInit();
        BtnBack.onClick.AddListener(() =>
        {
            Quitting?.Invoke();
            ServiceFactory.Instance.GetService<PanelManager>()
                .ClosePanel(this);
        });
    }

    public void SetOption(IEnumerable<Artifact> options)
    {
        foreach(var art in options)
        {
            var obj = Instantiate(ViewModel, Root);
            var view = obj.GetComponent<ArtifactView>();
            view.Binding(art);
            view.Clicked += () =>
            {
                Selected?.Invoke(art);
            };
            if (options.Count() >= 1)
            {
                ArcLayout.Children.Add(view);
            }
        }
        ArcLayout.Refresh();
    }

    [Sirenix.OdinInspector.Button]
    void Test(ArtifactView view)
    {
        ArcLayout.Children.Add(view);
        ArcLayout.Refresh();
    }
}
