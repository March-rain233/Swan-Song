using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameToolKit.Editor {
    public abstract class NodeSearchProviderBase : ScriptableObject, ISearchWindowProvider
    {
        public abstract List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context);

        public abstract bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context);
    }
}
