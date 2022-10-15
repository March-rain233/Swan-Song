using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �ӵ�
/// </summary>
/// <remarks>
/// ����������ƶ�
/// </remarks>
public class ViewPoint : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if(Mouse.current.rightButton.isPressed)
        {
            var p = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            p.z = 0;
            transform.position = p;
        }
    }
}
