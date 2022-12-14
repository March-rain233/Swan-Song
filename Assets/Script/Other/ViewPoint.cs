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
    public float MaxVelocity = 0.5f;
    public Rect Border;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (Pointer.current.press.isPressed)
        {
            var delta = Pointer.current.delta.ReadValue().normalized;
            delta = -delta * Mathf.Lerp(0, MaxVelocity, delta.magnitude);

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
