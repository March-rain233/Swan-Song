using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolKit;
using GameToolKit.Utility;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Sirenix.Serialization;
using DG.Tweening;

public class TreeMapView : PanelBase
{
    public Vector4 Margin;

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
            view.localPosition = data.Position + new Vector2(_view.Margin.x, _view.Margin.y);
        }
    }

    public override PanelShowType ShowType => PanelShowType.Normal;
    Dictionary<int, RectTransform> _nodes = new Dictionary<int, RectTransform>();
    TreeMap _map;
    public Material LineMaterial;
    public Gradient LineColor;
    public Gradient AvaliableLineColor;
    public Gradient PathLineColor;
    public RectTransform Root;
    public LineRendererUGUI LineRendererUGUI;

    protected override void OnInit()
    {
        base.OnInit();
        _map = ServiceFactory.Instance.GetService<GameManager>().GameData.TreeMap;
        var nodeModel = UISetting.Instance.PrefabsDic["TreeMapNode"];
        //创建节点
        foreach (var id in _map.Nodes)
        {
            var node = _map.FindNode(id);
            var nodeView = Instantiate(nodeModel, Root);
            nodeView.name = $"{id}";
            nodeView.GetComponentInChildren<TMPro.TextMeshProUGUI>().text 
                = node.PlaceType.GetDescription();
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
            //nodeView.DOScale(new Vector3(1.1f, 1.1f, 1), BattleAnimator.LongAnimationDuration)
            //    .SetLoops(-1, LoopType.Yoyo);
        }
        //设定节点位置
        float actualWidth, actualHeight;
        GraphLayoutUtility.HierarchicalLayout(new GraphLayoutAdapter(this), _map.RootId,
            actualWidth: out actualWidth,  actualHeight: out actualHeight,
            intervalWidth:70, intervalHeight:40);
        //扩展画布宽度
        actualWidth += Margin.x + Margin.z;
        actualHeight += Margin.y + Margin.w;
        Root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, actualWidth);
        Root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, actualHeight);
        foreach (var node in _nodes.Values)
        {
            UIUtility.SetAnchor(node);
        }
        var arf =Root.gameObject.AddComponent<AspectRatioFitter>();
        arf.aspectRatio = actualWidth / actualHeight;
        arf.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        Root.sizeDelta = new Vector2(Root.sizeDelta.x, 0);
        Root.ForceUpdateRectTransforms();
        //设定节点间连线
        LineRendererUGUI.material = LineMaterial;
        foreach (var node in _nodes)
        {
            var children = _map.GetChildren(node.Key);
            foreach (var childIndex in children)
            {
                var child = _nodes[childIndex];
                var line = new UISegment();
                line.LineColor = node.Key == _map.CurrentId ? AvaliableLineColor : 
                    _map.Path.Contains(node.Key) && _map.Path.Contains(childIndex) ? PathLineColor: LineColor;
                line.WidthCruve = new AnimationCurve(new Keyframe(0, 5));
                line.StarTransform = node.Value;
                line.EndTransform = child;
                LineRendererUGUI.Lines.Add(line);
            }
        }
        LineRendererUGUI.Refresh();
        //foreach (var node in _nodes)
        //{
        //    var children = _map.GetChildren(node.Key);
        //    foreach (var child in children)
        //    {
        //        var obj = new GameObject("Line", typeof(RectTransform), typeof(LineRenderer));
        //        obj.layer = LayerMask.NameToLayer("UI");
        //        obj.transform.SetParent(node.Value, false);
        //        var line = obj.GetComponent<LineRenderer>();
        //        line.material = LineMaterial;
        //        line.colorGradient = LineColor;
        //        line.useWorldSpace = false;
        //        line.widthMultiplier = 0.5f;
        //        line.SetPositions(new Vector3[2] {
        //            Vector3.zero,
        //            node.Value.InverseTransformPoint(_nodes[child].position)
        //        });
        //    }
        //}
    }
}
