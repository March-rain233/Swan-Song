using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
public class Test : SerializedMonoBehaviour
{
    public ArcLayout ArcLayout;

    private void OnValidate()
    {
        ArcLayout.Children = transform.GetComponentsInChildren<CardView>().OfType<IArcLayoutElement>().ToList();
        ArcLayout.Refresh();
    }
}
