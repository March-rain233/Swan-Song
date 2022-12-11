using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameToolKit
{
    public class PageGroup : MonoBehaviour
    {
        public List<RectTransform> PageList;
        public Button BtnNext;
        public Button BtnPrevious;

        public float AnimDuration = 0.1f;
        public int Index
        {
            get;
            private set;
        } = 0;

        public int PageCount => PageList.Count;

        private void Start()
        {
            SetPage(0);
            BtnNext.onClick.AddListener(NextPage);
            BtnPrevious.onClick.AddListener(PrePage);
        }

        public void NextPage()
        {
            SetPage(Index + 1);
        }

        public void PrePage()
        {
            SetPage(Index - 1);
        }

        public void SetPage(int index)
        {
            if (index >= 0 && index < PageCount)
            {
                PageList[Index].gameObject.SetActive(false);
                PageList[index].gameObject.SetActive(true);
                Index = index;
                RefreshButton();
            }
        }

        void RefreshButton()
        {
            BtnPrevious.interactable = Index > 0;
            BtnNext.interactable = Index < PageCount - 1;
        }
    }
}
