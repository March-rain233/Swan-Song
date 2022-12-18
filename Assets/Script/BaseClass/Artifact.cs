using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 遗物
/// </summary>
public abstract class Artifact
{
    [JsonConverter(typeof(ObjectConvert))]
    public Sprite Sprite;

    public string Name;

    [TextArea]
    public string Description;

    public void Enable()
    {
        OnEnable();
    }

    protected abstract void OnEnable();

    public void Disable()
    {
        OnDisable();
    }

    protected abstract void OnDisable();
}