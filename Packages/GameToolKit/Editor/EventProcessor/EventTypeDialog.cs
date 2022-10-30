using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using System;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;
namespace GameToolKit.EventProcessor.Editor
{
    public class EventTypeDialog : OdinEditorWindow
    {
        [ValueDropdown("GetValidList")]
        public Type EventType;
        public string EventName;
        [FolderPath(RequireExistingPath = true)]
        [ShowIf("ShowFolderPath")]
        public string FolderPath;
        [HideInInspector]
        public bool ShowFolderPath = true;
        [HideInInspector]
        public Action<Type, string> Callback;
        [Button(Name = "Confirm")]
        public void Confirm()
        {
            Close();
            Callback?.Invoke(EventType, EventName);
        }
        public IEnumerable<Type> GetValidList()
        {
            var temp = TypeCache.GetTypesDerivedFrom(typeof(EventBase));
            return temp.Where(t => !t.IsAbstract && !t.IsInterface);
        }
    }
}

