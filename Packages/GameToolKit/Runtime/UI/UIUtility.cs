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
    }
}
