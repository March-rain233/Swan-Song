using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameToolKit
{
    /// <summary>
    /// 面板基类
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PanelBase : SerializedMonoBehaviour
    {
        /// <summary>
        /// 是否在关闭的时候摧毁
        /// </summary>
        [ReadOnly, ShowInInspector]
        public virtual bool IsDestoryOnClosed => true;

        [ReadOnly, ShowInInspector]
        public virtual PanelShowType ShowType => PanelShowType.Normal;

        [ReadOnly, SerializeField]
        protected CanvasGroup CanvasGroup;

        public bool BlocksRaycasts
        {
            get { return CanvasGroup.blocksRaycasts; }
            set { CanvasGroup.blocksRaycasts = value; }
        }

        public bool Interactable
        {
            get { return CanvasGroup.interactable; }
            set { CanvasGroup.interactable = value; }
        }

        public float Alpha
        {
            get { return CanvasGroup.alpha; }
            set { CanvasGroup.alpha = value; }
        }

        /// <summary>
        /// 当前窗口状态
        /// </summary>

        public bool IsShowing { get; private set; } = false;

        private void OnValidate()
        {
            CanvasGroup = CanvasGroup ?? GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// 当面板数据初始化
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// 当面板被打开时
        /// </summary>
        protected virtual void OnOpen() { gameObject.SetActive(true); }

        internal void Open()
        {
            IsShowing = true;
            OnInit();
            OnOpen();
        }

        /// <summary>
        /// 当面板被关闭时
        /// </summary>
        /// <remarks>
        /// 请在动画结束后调用Dispose
        /// </remarks>
        protected virtual void OnClose() 
        { 
            gameObject.SetActive(false); 
            Dispose();
        }

        internal void Close()
        {
            IsShowing = false;
            OnClose();
        }

        /// <summary>
        /// 当面板显示时
        /// </summary>
        protected virtual void OnShow() { gameObject.SetActive(true);}

        internal void Show()
        {
            if (!IsShowing)
            {
                IsShowing = true;
                OnShow();
            }
        }

        /// <summary>
        /// 当面板隐藏时
        /// </summary>
        protected virtual void OnHide() { gameObject?.SetActive(false); }

        internal void Hide()
        {
            if (IsShowing)
            {
                IsShowing = false;
                OnHide();
            }
        }

        /// <summary>
        /// 当面板被摧毁或移入池中时
        /// </summary>
        /// <remarks>
        /// 负责关闭面板后的数据处理
        /// </remarks>
        protected virtual void OnDispose() { }

        /// <summary>
        /// 立即摧毁
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            if (IsDestoryOnClosed)
            {
                Destroy(gameObject);
            }
            else
            {
                ServiceFactory.Instance.GetService<PanelManager>().RecyclePanel(this);
            }
        }
    }

    [Flags]
    public enum PanelShowType
    {
        /// <summary>
        /// 与多个面板叠加
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 反向切换
        /// </summary>
        /// <remarks>
        /// 将在开启时隐藏上一个拥有该标签的面板，在关闭时恢复上一个拥有该标签的面板
        /// </remarks>
        ReverseChange = 1,
        /// <summary>
        /// 隐藏所有
        /// </summary>
        /// <remarks>
        /// 在开启时将隐藏从此面板到上一个拥有该标签的面板之间的所有面板，关闭时反向
        /// </remarks>
        HideOther = ReverseChange << 1,
    }
}
