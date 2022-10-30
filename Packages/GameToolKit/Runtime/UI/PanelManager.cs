using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameToolKit
{
    public class PanelManager : IService
    {
        /// <summary>
        /// UI根节点
        /// </summary>
        public Canvas Root { get; private set; }

        /// <summary>
        /// 保持开启状态的面板根节点
        /// </summary>
        public RectTransform ActiveRoot { get; private set; }

        /// <summary>
        /// 进入关闭状态的面板根节点
        /// </summary>
        public RectTransform DeathRoot { get; private set; }

        /// <summary>
        /// UI摄像机
        /// </summary>
        public Camera UICamera { get; private set; }

        /// <summary>
        /// UI事件系统
        /// </summary>
        public EventSystem EventSystem { get; private set; }

        /// <summary>
        /// 开启状态的面板列表
        /// </summary>
        [ShowInInspector, ReadOnly]
        List<PanelBase> _openPanelList = new List<PanelBase>();

        void IService.Init()
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            //根节点生成
            Root = new GameObject("UIRoot", typeof(Canvas), typeof(UnityEngine.UI.GraphicRaycaster), typeof(UnityEngine.UI.CanvasScaler)).GetComponent<Canvas>();
            DeathRoot = new GameObject("DeathRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            DeathRoot.SetParent(Root.transform, false);
            ActiveRoot = new GameObject("ActiveRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            ActiveRoot.SetParent(Root.transform, false);

            //相机设定
            UICamera = new GameObject("UICamera", typeof(Camera)).GetComponent<Camera>();
            UICamera.cullingMask = LayerMask.GetMask("UI");
            UICamera.orthographic = true;
            UICamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(UICamera);

            //事件系统生成
            EventSystem = new GameObject("EventSystem", typeof(EventSystem), 
                typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule))
                .GetComponent<EventSystem>();

            //根画布设定
            Root.worldCamera = UICamera;
            Root.renderMode = RenderMode.ScreenSpaceCamera;
            Root.sortingLayerName = "UI";

            Root.gameObject.layer = uiLayer;
            ActiveRoot.gameObject.layer = uiLayer;
            DeathRoot.gameObject.layer = uiLayer;

            ActiveRoot.offsetMax = Vector2.zero;
            ActiveRoot.offsetMin = Vector2.zero;
            ActiveRoot.anchorMax = Vector2.one;
            ActiveRoot.anchorMin = Vector2.zero;

            DeathRoot.gameObject.SetActive(false);

            //设置为持久化物体
            Object.DontDestroyOnLoad(Root.gameObject);
            Object.DontDestroyOnLoad(UICamera);
            Object.DontDestroyOnLoad(EventSystem);
        }

        /// <summary>
        /// 开启面板
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PanelBase OpenPanel(string name)
        {
            var panel = CreatePanel(name);
            panel.transform.SetParent(ActiveRoot, false);

            //如果存在ReverseChange，则查找到距离最近的拥有该标签的窗面板并关闭
            if (panel.ShowType.HasFlag(PanelShowType.ReverseChange))
            {
                _openPanelList.FindLast(e=>e.ShowType.HasFlag(PanelShowType.ReverseChange))?.Hide();
            }
            //如果存在HideOther，则关闭到上一个拥有该标签的面板为止的所有面板
            if (panel.ShowType.HasFlag(PanelShowType.HideOther))
            {
                foreach(var p in _openPanelList.Reverse<PanelBase>().TakeWhile(e => !e.ShowType.HasFlag(PanelShowType.HideOther)))
                {
                    p.Hide();
                }
            }

            _openPanelList.Add(panel);
            panel.Open();
            return panel;
        }

        /// <summary>
        /// 关闭最新生成的同名面板
        /// </summary>
        /// <param name="name"></param>
        public void ClosePanel(string name)
        {
            ClosePanel(_openPanelList.FindLast(e=>e.name == name));
        }

        /// <summary>
        /// 关闭指定面板
        /// </summary>
        /// <param name="panel"></param>
        public void ClosePanel(PanelBase panel)
        {
            _openPanelList.Remove(panel);
            panel.Close();

            //如果存在ReverseChange，则查找到距离最近的拥有该标签的窗面板开启
            if (panel.ShowType.HasFlag(PanelShowType.ReverseChange))
            {
                _openPanelList.FindLast(e => e.ShowType.HasFlag(PanelShowType.ReverseChange)).Show();
            }
            //如果存在HideOther，则开启到上一个拥有该标签的面板为止的所有面板
            if (panel.ShowType.HasFlag(PanelShowType.HideOther))
            {
                foreach (var p in _openPanelList.Reverse<PanelBase>().TakeWhile(e => !e.ShowType.HasFlag(PanelShowType.HideOther)))
                {
                    p.Show();
                }
            }
        }

        /// <summary>
        /// 获取已开启的面板，若当前开启面板中不存在则创建新面板
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PanelBase GetOrOpenPanel(string name)
        {
            PanelBase panel;
            if(!TryGetPanel(name, out panel))
            {
                panel = OpenPanel(name);
            }
            return panel;
        }

        /// <summary>
        /// 尝试获取已开启的面板
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool TryGetPanel(string name, out PanelBase panel)
        {
            panel = _openPanelList.FindLast(e => e.name == name);
            return panel != null;
        }

        /// <summary>
        /// 创建面板
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        PanelBase CreatePanel(string name)
        {
            var res = DeathRoot.Find(name);
            if(res == null)
            {
                res = Object.Instantiate(UISetting.Instance.PrefabsDic[name].gameObject, DeathRoot).transform;
                res.name = name;
                res.gameObject.layer = LayerMask.NameToLayer("UI");
            }
            return res.GetComponent<PanelBase>();
        }

        /// <summary>
        /// 回收面板
        /// </summary>
        /// <param name="panel"></param>
        internal void RecyclePanel(PanelBase panel)
        {
            panel.transform.SetParent(DeathRoot, false);
        }
    }
}
