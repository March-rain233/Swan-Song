using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 镂空遮罩
/// </summary>
public class HollowOutMask : Graphic, ICanvasRaycastFilter
{
    //镂空区域
    public RectTransform m_HollowOutArea;
    public void SetHollowOutArea(RectTransform hollowOutArea)
    {
        m_HollowOutArea = hollowOutArea;
    }

    Vector2 m_HollowOutArea_RT;
    Vector2 m_HollowOutArea_LB;

    public bool IsPenetrate = true;

    /// <summary>
    /// 强制更新
    /// </summary>
    public void ForceUpdate()
    {
        DoUpdate();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (m_HollowOutArea == null)
        {
            //如果没有镂空区域则绘制全屏遮罩
            base.OnPopulateMesh(vh);
            return;
        }

        vh.Clear();
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector2(rectTransform.rect.min.x, rectTransform.rect.min.y);
        vh.AddVert(vertex);
        vertex.position = new Vector2(rectTransform.rect.max.x, rectTransform.rect.min.y);
        vh.AddVert(vertex);
        vertex.position = new Vector2(rectTransform.rect.max.x, rectTransform.rect.max.y);
        vh.AddVert(vertex);
        vertex.position = new Vector2(rectTransform.rect.min.x, rectTransform.rect.max.y);
        vh.AddVert(vertex);

        vertex.position = new Vector2(m_HollowOutArea_LB.x, m_HollowOutArea_LB.y);
        vh.AddVert(vertex);
        vertex.position = new Vector2(m_HollowOutArea_RT.x, m_HollowOutArea_LB.y);
        vh.AddVert(vertex);
        vertex.position = new Vector2(m_HollowOutArea_RT.x, m_HollowOutArea_RT.y);
        vh.AddVert(vertex);
        vertex.position = new Vector2(m_HollowOutArea_LB.x, m_HollowOutArea_RT.y);
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 4);
        vh.AddTriangle(1, 4, 5);
        vh.AddTriangle(1, 5, 2);
        vh.AddTriangle(2, 5, 6);
        vh.AddTriangle(2, 6, 7);
        vh.AddTriangle(2, 3, 7);
        vh.AddTriangle(3, 7, 0);
        vh.AddTriangle(0, 4, 7);
    }

    private void Update()
    {
        DoUpdate();
    }

    void DoUpdate()
    {
        if (m_HollowOutArea == null)
        {
            return;
        }

        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform, m_HollowOutArea);
        if (m_HollowOutArea_RT != (Vector2)bounds.max
            || m_HollowOutArea_LB != (Vector2)bounds.min)
        {
            m_HollowOutArea_RT = bounds.max;
            m_HollowOutArea_LB = bounds.min;
            SetAllDirty();
        }
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (m_HollowOutArea == null || !IsPenetrate)
        {
            return true;
        }

        bool contain = RectTransformUtility.RectangleContainsScreenPoint(m_HollowOutArea, sp, eventCamera);
        return !contain;
    }
}
