using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// 线条渲染器
    /// </summary>
    public class LineRendererUGUI : MaskableGraphic
    {
        public List<Line> Lines = new List<Line>();
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            foreach(var line in Lines)
            {
                line.ConstructLineMesh(vh);
            }
        }

        [Button]
        public void Refresh()
        {
            SetVerticesDirty();
        }
    }

    /// <summary>
    /// 线条构筑器
    /// </summary>
    public abstract class Line
    {
        /// <summary>
        /// 头尾端点处使用的顶点个数
        /// </summary>
        [Range(0, 90)]
        public int EndPointVertexNum;

        /// <summary>
        /// 构筑线段网络
        /// </summary>
        /// <param name="vh"></param>
        public virtual void ConstructLineMesh(VertexHelper vh)
        {
            var list = PositionsPreprocess();
            ConstructLine(list, vh);
        }
        /// <summary>
        /// 渲染点集预处理
        /// </summary>
        /// <returns></returns>
        protected abstract List<Vector3> PositionsPreprocess();

        protected abstract void ConstructLine(List<Vector3> positions, VertexHelper vh);
        protected static Vector3 GetVerticalDirection(Vector3 start, Vector3 end)
        {
            return Vector3.Cross(start - end, Vector3.forward).normalized;
        }
        public static float GetLength(List<Vector3> positions)
        {
            float length = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                length += Vector3.Distance(positions[i], positions[i - 1]);
            }
            return length;
        }
    }

    /// <summary>
    /// 线段
    /// </summary>
    public abstract class LineSegment : Line
    {
        public abstract Vector3 StartPosition { get; }
        public abstract Vector3 EndPosition { get; }

        /// <summary>
        /// 线宽曲线
        /// </summary>
        public AnimationCurve WidthCruve;

        /// <summary>
        /// 线条颜色
        /// </summary>
        public Gradient LineColor;

        protected override void ConstructLine(List<Vector3> positions, VertexHelper vh)
        {
            var vertices = new List<UIVertex>();
            var indices = new List<int>();
            var start = positions[0];
            var end = positions[1];
            var lineDir = end - start;
            var verticalDir = GetVerticalDirection(start, end);
            var startColor = LineColor.Evaluate(0);
            var endColor = LineColor.Evaluate(1);
            var startHalfWidth = WidthCruve.Evaluate(0) * 0.5f;
            var endHalfWidth = WidthCruve.Evaluate(1) * 0.5f;
            var normal = Vector3.Cross(lineDir, verticalDir);

            vertices.Add(new UIVertex()
            {
                position = start + verticalDir * startHalfWidth,
                color = startColor,
                normal = normal
            });
            vertices.Add(new UIVertex()
            {
                position = start - verticalDir * startHalfWidth,
                color = startColor,
                normal = normal
            });
            vertices.Add(new UIVertex()
            {
                position = end + verticalDir * startHalfWidth,
                color = endColor,
                normal = normal
            });
            vertices.Add(new UIVertex()
            {
                position = end - verticalDir * startHalfWidth,
                color = endColor,
                normal = normal
            });

            indices.Add(1);
            indices.Add(0);
            indices.Add(2);

            indices.Add(2);
            indices.Add(3);
            indices.Add(1);

            //{
            //    vertexs.Add(new UIVertex()
            //    {
            //        position = start,
            //        color = startColor,
            //        normal = normal
            //    });
            //    var center = 4;
            //    var delta = 180 / EndPointVertexNum;
            //    for(int i = 1; i < EndPointVertexNum; i++)
            //    {
            //        var dir = Quaternion.AngleAxis(i * delta, normal) * verticalDir;
            //        vertexs.Add(new UIVertex()
            //        {
            //            position = start + dir * startHalfWidth,
            //            color = startColor,
            //            normal = normal
            //        });

            //    }
            //}

            int vertexCount = vh.currentVertCount;
            foreach (var vertex in vertices)
            {
                vh.AddVert(vertex);
            }
            for (int i = 0; i < indices.Count; i += 3)
            {
                vh.AddTriangle(indices[i] + vertexCount, indices[i + 1] + vertexCount, indices[i + 2] + vertexCount);
            }
        }

        protected override List<Vector3> PositionsPreprocess()
        {
            return new List<Vector3>() { StartPosition, EndPosition };
        }
    }

    public class UISegment : LineSegment
    {
        public RectTransform StarTransform;
        public RectTransform EndTransform;

        public override Vector3 StartPosition => StarTransform.localPosition;

        public override Vector3 EndPosition => EndTransform.localPosition;
    }

    /// <summary>
    /// 折线
    /// </summary>
    [Serializable]
    public class BrokenLine : Line
    {
        /// <summary>
        /// 点集
        /// </summary>
        public List<Vector3> Positions = new() { };

        /// <summary>
        /// 转角处使用的顶点个数
        /// </summary>
        [Range(0, 90)]
        public int CornerVertexNum;

        /// <summary>
        /// 线宽曲线
        /// </summary>
        public AnimationCurve WidthCruve;

        /// <summary>
        /// 线条颜色
        /// </summary>
        public Gradient LineColor;

        public override void ConstructLineMesh(VertexHelper vh)
        {
            var list = PositionsPreprocess();
            if (list.Count >= 2)
            {
                ConstructLine(list, vh);
            }
        }

        protected override List<Vector3> PositionsPreprocess()
        {
            return Positions;
        }

        protected override void ConstructLine(List<Vector3> positions, VertexHelper vh)
        {
            List<UIVertex> vertices = new List<UIVertex>();
            List<int> indices = new List<int>();
            float totalLength = GetLength(positions);
            float length = 0;

            int preR = 0, preL = 0;
            //处理起点
            {
                var dirL = GetVerticalDirection(positions[0], positions[1]);
                var color = LineColor.Evaluate(0);
                var halfWidth = WidthCruve.Evaluate(0) * 0.5f;
                vertices.Add(new UIVertex()
                {
                    color = color,
                    position = positions[0] + dirL * halfWidth,
                });
                vertices.Add(new UIVertex()
                {
                    color = color,
                    position = positions[0] - dirL * halfWidth,
                });
                preL = 0;
                preR = 1;
            }
            //处理折点
            for (int i = 1; i < positions.Count - 1; i++)
            {
                int curL = 0, curR = 0;
                Vector3 pre = positions[i - 1];
                Vector3 cur = positions[i];
                Vector3 next = positions[i + 1];
                length += Vector3.Distance(cur, pre);
                float t = length / totalLength;
                var color = LineColor.Evaluate(t);
                var width = WidthCruve.Evaluate(t);
                var halfWidth = width * 0.5f;
                var dirPre = GetVerticalDirection(pre, cur);
                var dirNext = GetVerticalDirection(cur, next);
                var angle = Vector3.Angle(dirPre, dirNext);
                var r = halfWidth / Mathf.Cos(angle * Mathf.Deg2Rad * 0.5f);
                var innerDir = ((pre - cur).normalized + (next - cur).normalized).normalized;
                var innerPos = cur + innerDir * r;
                vertices.Add(new UIVertex()
                {
                    color = color,
                    normal = Vector3.Cross(dirPre, cur - pre),
                    position = innerPos,
                    tangent = cur - pre,
                });
                bool sign = Vector3.Dot(pre - cur, dirNext) > 0;
                if (sign)
                {
                    curL = vertices.Count - 1;
                }
                else
                {
                    curR = vertices.Count - 1;
                }

                if (CornerVertexNum == 0)
                {
                    vertices.Add(new UIVertex()
                    {
                        normal = Vector3.Cross(dirNext, next - cur),
                        color = color,
                        position = innerDir * -r + cur,
                        tangent = next - cur,
                    });
                    if (sign)
                    {
                        curR = vertices.Count - 1;
                    }
                    else
                    {
                        curL = vertices.Count - 1;
                    }

                    indices.Add(preR);
                    indices.Add(preL);
                    indices.Add(curL);

                    indices.Add(curL);
                    indices.Add(curR);
                    indices.Add(preR);
                }
                else
                {
                    Vector3 start, end;
                    if (sign)
                    {
                        curR = vertices.Count + CornerVertexNum;
                        start = -dirPre;
                        end = -dirNext;
                        for (int j = 0; j <= CornerVertexNum; j++)
                        {
                            var dir = Vector3.Lerp(start, end, j / (CornerVertexNum * 1.0f));
                            vertices.Add(new UIVertex()
                            {
                                color = color,
                                position = innerPos + dir * width,
                            });
                        }
                        for (int j = 0; j < CornerVertexNum; j++)
                        {
                            indices.Add(curL);
                            indices.Add(vertices.Count - CornerVertexNum + j);
                            indices.Add(vertices.Count - CornerVertexNum + j - 1);
                        }
                        indices.Add(preR);
                        indices.Add(preL);
                        indices.Add(curL);

                        indices.Add(curL);
                        indices.Add(vertices.Count - CornerVertexNum - 1);
                        indices.Add(preR);
                    }
                    else
                    {
                        curL = vertices.Count + CornerVertexNum;
                        start = dirPre;
                        end = dirNext;

                        for (int j = 0; j <= CornerVertexNum; j++)
                        {
                            var dir = Vector3.Lerp(start, end, j / (CornerVertexNum * 1.0f)).normalized;
                            vertices.Add(new UIVertex()
                            {
                                color = color,
                                position = innerPos + dir * width,
                            });

                        }
                        for (int j = 0; j < CornerVertexNum; j++)
                        {
                            indices.Add(curR);
                            indices.Add(vertices.Count - CornerVertexNum + j - 1);
                            indices.Add(vertices.Count - CornerVertexNum + j);
                        }
                        indices.Add(preR);
                        indices.Add(preL);
                        indices.Add(vertices.Count - CornerVertexNum - 1);

                        indices.Add(vertices.Count - CornerVertexNum - 1);
                        indices.Add(curR);
                        indices.Add(preR);
                    }
                }

                preL = curL;
                preR = curR;
            }
            //处理终点
            {
                var dirL = GetVerticalDirection(positions[positions.Count - 2], positions[positions.Count - 1]);
                var color = LineColor.Evaluate(1);
                var halfWidth = WidthCruve.Evaluate(1) * 0.5f;
                vertices.Add(new UIVertex()
                {
                    color = color,
                    position = positions[positions.Count - 1] + dirL * halfWidth,
                });
                vertices.Add(new UIVertex()
                {
                    color = color,
                    position = positions[positions.Count - 1] - dirL * halfWidth,
                });
                int curL = vertices.Count - 2;
                int curR = vertices.Count - 1;

                indices.Add(preR);
                indices.Add(preL);
                indices.Add(curL);

                indices.Add(curL);
                indices.Add(curR);
                indices.Add(preR);
            }

            int vertexCount = vh.currentVertCount;
            foreach (var vertex in vertices)
            {
                vh.AddVert(vertex);
            }
            for (int i = 0; i < indices.Count; i += 3)
            {
                vh.AddTriangle(indices[i] + vertexCount, indices[i + 1] + vertexCount, indices[i + 2] + vertexCount);
            }
        }
    }
}
