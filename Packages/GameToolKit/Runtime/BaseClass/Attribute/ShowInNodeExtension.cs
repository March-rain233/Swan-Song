using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameToolKit
{
    /// <summary>
    /// ��ʾ�ڽڵ���չ��
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowInNodeExtension : Attribute
    {
    }
}
