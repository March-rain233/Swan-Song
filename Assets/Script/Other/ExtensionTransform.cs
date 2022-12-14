using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ExtensionTransform
{
    public static Transform FindAll(this Transform child, string name)//child 是要查询的物体的transfrom，name是要查询的子物体的名字
    {
        //假设在当前层级查找子对象
        Transform it = child.Find(name);//定义一个  Transform 变量 it  用来接收查询到子物体
        if (it != null)
        {
            return it;
        }
        for (int i = 0; i < child.childCount; i++)//假设查找子对象的子对象
        {
            Transform its = FindAll(child.GetChild(i), name);//调用方法名进行递归  its含义也是定义一个  Transform 变量 it s 用来接收查询到子物体
            if (its != null)//如果不为空，就是存在次物体返回 该物体
            {
                return its;
            }
        }
        return null;//如果不存在返回空
    }
}

