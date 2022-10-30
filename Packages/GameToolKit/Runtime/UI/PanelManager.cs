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
        /// UI���ڵ�
        /// </summary>
        public Canvas Root { get; private set; }

        /// <summary>
        /// ���ֿ���״̬�������ڵ�
        /// </summary>
        public RectTransform ActiveRoot { get; private set; }

        /// <summary>
        /// ����ر�״̬�������ڵ�
        /// </summary>
        public RectTransform DeathRoot { get; private set; }

        /// <summary>
        /// UI�����
        /// </summary>
        public Camera UICamera { get; private set; }

        /// <summary>
        /// UI�¼�ϵͳ
        /// </summary>
        public EventSystem EventSystem { get; private set; }

        /// <summary>
        /// ����״̬������б�
        /// </summary>
        [ShowInInspector, ReadOnly]
        List<PanelBase> _openPanelList = new List<PanelBase>();

        void IService.Init()
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            //���ڵ�����
            Root = new GameObject("UIRoot", typeof(Canvas), typeof(UnityEngine.UI.GraphicRaycaster), typeof(UnityEngine.UI.CanvasScaler)).GetComponent<Canvas>();
            DeathRoot = new GameObject("DeathRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            DeathRoot.SetParent(Root.transform, false);
            ActiveRoot = new GameObject("ActiveRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            ActiveRoot.SetParent(Root.transform, false);

            //����趨
            UICamera = new GameObject("UICamera", typeof(Camera)).GetComponent<Camera>();
            UICamera.cullingMask = LayerMask.GetMask("UI");
            UICamera.orthographic = true;
            UICamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(UICamera);

            //�¼�ϵͳ����
            EventSystem = new GameObject("EventSystem", typeof(EventSystem), 
                typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule))
                .GetComponent<EventSystem>();

            //�������趨
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

            //����Ϊ�־û�����
            Object.DontDestroyOnLoad(Root.gameObject);
            Object.DontDestroyOnLoad(UICamera);
            Object.DontDestroyOnLoad(EventSystem);
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PanelBase OpenPanel(string name)
        {
            var panel = CreatePanel(name);
            panel.transform.SetParent(ActiveRoot, false);

            //�������ReverseChange������ҵ����������ӵ�иñ�ǩ�Ĵ���岢�ر�
            if (panel.ShowType.HasFlag(PanelShowType.ReverseChange))
            {
                _openPanelList.FindLast(e=>e.ShowType.HasFlag(PanelShowType.ReverseChange))?.Hide();
            }
            //�������HideOther����رյ���һ��ӵ�иñ�ǩ�����Ϊֹ���������
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
        /// �ر��������ɵ�ͬ�����
        /// </summary>
        /// <param name="name"></param>
        public void ClosePanel(string name)
        {
            ClosePanel(_openPanelList.FindLast(e=>e.name == name));
        }

        /// <summary>
        /// �ر�ָ�����
        /// </summary>
        /// <param name="panel"></param>
        public void ClosePanel(PanelBase panel)
        {
            _openPanelList.Remove(panel);
            panel.Close();

            //�������ReverseChange������ҵ����������ӵ�иñ�ǩ�Ĵ���忪��
            if (panel.ShowType.HasFlag(PanelShowType.ReverseChange))
            {
                _openPanelList.FindLast(e => e.ShowType.HasFlag(PanelShowType.ReverseChange)).Show();
            }
            //�������HideOther����������һ��ӵ�иñ�ǩ�����Ϊֹ���������
            if (panel.ShowType.HasFlag(PanelShowType.HideOther))
            {
                foreach (var p in _openPanelList.Reverse<PanelBase>().TakeWhile(e => !e.ShowType.HasFlag(PanelShowType.HideOther)))
                {
                    p.Show();
                }
            }
        }

        /// <summary>
        /// ��ȡ�ѿ�������壬����ǰ��������в������򴴽������
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
        /// ���Ի�ȡ�ѿ��������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool TryGetPanel(string name, out PanelBase panel)
        {
            panel = _openPanelList.FindLast(e => e.name == name);
            return panel != null;
        }

        /// <summary>
        /// �������
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
        /// �������
        /// </summary>
        /// <param name="panel"></param>
        internal void RecyclePanel(PanelBase panel)
        {
            panel.transform.SetParent(DeathRoot, false);
        }
    }
}
