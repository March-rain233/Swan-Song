using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 视点
/// </summary>
/// <remarks>
/// 控制相机的移动
/// </remarks>
public class ViewPoint : MonoBehaviour
{
    public Rect Border;
    Vector2 _lastPoint;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (Pointer.current.press.wasPressedThisFrame)
        {
            _lastPoint = Pointer.current.position.ReadValue();
        }
        else if(Pointer.current.press.isPressed)
        {
            var start = Camera.main.ScreenToWorldPoint(_lastPoint);
            var end = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
            _lastPoint = Pointer.current.position.ReadValue();
            var delta = start - end;

            var position = transform.position + (Vector3)delta;
            position.x = Mathf.Clamp(position.x, Border.xMin, Border.xMax);
            position.y = Mathf.Clamp(position.y, Border.yMin, Border.yMax);

            transform.position = position;
        }
    }

    public void ResetToCenter()
    {
        transform.position = Border.center;
    }
}
