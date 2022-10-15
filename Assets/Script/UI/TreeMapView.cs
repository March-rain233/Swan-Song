using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using GameToolKit.Utility;
using System.Linq;
using UnityEngine.UI;

public class TreeMapView : PanelBase
{
    class GraphLayoutAdapter : GameToolKit.Utility.GraphLayoutAdapter
    {
        TreeMapView _view;
        public GraphLayoutAdapter(TreeMapView view)
        {
            _view = view;
        }
        public override IEnumerable<int> Nodes => _view._map.Nodes;

        public override IEnumerable<int> GetDescendant(int id)
        {
            return _view._map.GetChildren(id);
        }

        public override NodeData GetNodeData(int id)
        {
            var view = _view._nodes[id];
            var nodeData = new NodeData();
            nodeData.Width = view.rect.width;
            nodeData.Height = view.rect.height;
            nodeData.Position = view.position;
            return nodeData;
        }

        public override IEnumerable<int> GetPrecursor(int id)
        {
            throw new System.NotImplementedException();
        }

        public override void SetNodeData(int id, NodeData data)
        {
            var view = _view._nodes[id];
            view.localPosition = data.Position - new Vector2(((RectTransform)_view.transform).rect.width / 2, 0);
        }
    }
    public override PanelShowType ShowType => PanelShowType.Normal;
    Dictionary<int, RectTransform> _nodes = new Dictionary<int, RectTransform>();
    TreeMap _map;

    protected override void OnInit()
    {
        base.OnInit();
        _map = ServiceFactory.Instance.GetService<GameManager>().GameData.TreeMap;
        var nodeModel = UISetting.Instance.PrefabsDic["TreeMapNode"];
        //创建节点
        foreach (var id in _map.Nodes)
        {
            var node = _map.FindNode(id);
            var nodeView = Instantiate(nodeModel, transform);
            nodeView.GetComponentInChildren<TMPro.TextMeshProUGUI>().text 
                = System.Enum.GetName(node.PlaceType.GetType(), node.PlaceType);
            var btn = nodeView.GetComponent<Button>();
            btn.interactable = false;
            btn.onClick.AddListener(()=>(ServiceFactory.Instance.GetService<GameManager>()
                .GetState() as SelectLevelState).SelectNode(id));
            _nodes.Add(id, nodeView.GetComponent<RectTransform>());
        }
        foreach(var id in _map.GetChildren(_map.CurrentId))
        {
            var nodeView = _nodes[id];
            nodeView.GetComponent<Button>().interactable = true;
        }
        //设定节点位置
        GraphLayoutUtility.HierarchicalLayout(new GraphLayoutAdapter(this), _map.RootId);
    }
}
