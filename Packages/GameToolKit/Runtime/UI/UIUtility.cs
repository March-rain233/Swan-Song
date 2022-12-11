using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameToolKit
{
    /// <summary>
    /// UI工具类
    /// </summary>
    public static class UIUtility
    {
        public static void BindingData<TData>(
            IEnumerable<GameObject> views,
            IEnumerable<TData> source, 
            Func<GameObject> enter, 
            Action<GameObject> exit,
            Action<TData, GameObject, int> updata)
        {
            List<GameObject> newViews = new List<GameObject>(views);
            for(int i = views.Count(); i <= source.Count(); i++)
            {
                newViews.Add(enter());
            }
            for(int i = newViews.Count() - 1; i >= source.Count(); i--)
            {
                exit(newViews[i]);
                newViews.RemoveAt(newViews.Count() - 1);
            }
            for(int i = 0; i < source.Count(); i++)
            {
                updata(source.ElementAt(i), newViews[i], i);
            }
        }

        /// <summary>
        /// 设置锚点为组件的四角
        /// </summary>
        /// <param name="rectTransform"></param>
        public static void SetAnchor(RectTransform rectTransform)
        {
            var parentRect = rectTransform.parent as RectTransform;
            var parentSize = parentRect.rect.size;
            var offsetMin = rectTransform.offsetMin + (rectTransform.anchorMin * parentSize);
            var offsetMax = rectTransform.offsetMax + (rectTransform.anchorMax * parentSize);
            rectTransform.anchorMin = offsetMin / parentSize;
            rectTransform.anchorMax = offsetMax / parentSize;
            rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        }
    }
}
