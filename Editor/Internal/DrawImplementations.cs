using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Levers
{
    internal static class DrawImplementations
    {
        private static void UpdateTexture()
        {
            if (State.GradientFill == null)
            {
                Material.SetTexture("_MainTex", null);
            }
            else
            {
                Material.SetTexture("_MainTex", State.GradientFill.Texture);
            }
        }
        private class RenderState : IRenderState
        {
            public Color Fill { get; set; } = Color.white;
            public Color Stroke { get; set; } = Color.black;
            public IGradient GradientFill { get; set; } = null;
            public int StrokeWeight { get; set; } = 1;

            public RenderState()
            {

            }
        }
        private const string _unityUiClipRectKeyword = "UNITY_UI_CLIP_RECT";
        private const string _clipRectVariableName = "_ClipRect";
        private static PropertyInfo _visibleRectProperty;
        private static Rect? GetVisibleRect()
        {
            if (_visibleRectProperty == null)
            {
                System.Type guiClipType = typeof(GUI).Assembly.GetType("UnityEngine.GUIClip");
                if (guiClipType != null)
                {
                    _visibleRectProperty = guiClipType.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                }
            }
            var res = _visibleRectProperty == null ? null : (Rect?)_visibleRectProperty.GetValue(null, null);
            // Debug.Log("VisibleRect: " + res);
            return res;
        }
        private static void UpdateClipRect()
        {
            Rect? visibleRect = GetVisibleRect();
            if (visibleRect != null)
            {
                var vr = visibleRect.Value;
                var vect = new Vector4(vr.xMin, vr.yMin, vr.xMax, vr.yMax);
                Material.SetVector(_clipRectVariableName, vect);
                if (!Material.IsKeywordEnabled(_unityUiClipRectKeyword))
                {
                    Material.EnableKeyword(_unityUiClipRectKeyword);
                }
            }
            else
            {
                Material.SetVector(_clipRectVariableName, new Vector4(0, 0, Screen.width, Screen.height));
                if (Material.IsKeywordEnabled(_unityUiClipRectKeyword))
                {
                    Material.DisableKeyword(_unityUiClipRectKeyword);
                }
            }
        }
        private static Material __material;
        private static Material Material
        {
            get
            {
                if (__material == null)
                {
                    var shader = Shader.Find("Hidden/Com/Amequus/Levers/Draw");
                    __material = new Material(shader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }
                return __material;
            }
        }
        private static void DrawThickPolyline(float width, int actualNumberOfPoints, IReadOnlyList<Vector2> vertices)
        {
            if (vertices == null || vertices.Count < 2)
            {
                Debug.LogWarning("Polyline must have at least two points.");
                return;
            }

            DrawSetup(GL.TRIANGLE_STRIP, State.Stroke);
            var count = Mathf.Min(Mathf.Max(0, actualNumberOfPoints), vertices.Count);
            for (int i = 0; i < count; i++)
            {
                Vector2 CalcNormalA()
                {
                    Vector2 pointA = vertices[i - 1];
                    Vector2 pointB = vertices[i];

                    Vector2 directionA = (pointB - pointA).normalized;

                    return new Vector2(-directionA.y, directionA.x);
                }
                Vector2 CalcNormalB()
                {
                    Vector2 pointB = vertices[i];
                    Vector2 pointC = vertices[i + 1];

                    Vector2 directionB = (pointC - pointB).normalized;

                    return new Vector2(-directionB.y, directionB.x);
                }
                if (i == 0)
                {
                    Vector2 pointB = vertices[i];

                    Vector2 normalB = CalcNormalB();

                    Vector2 vertex1 = pointB + (normalB * width);
                    Vector2 vertex2 = pointB - (normalB * width);

                    AddPoint(vertex1);
                    AddPoint(vertex2);
                }
                else if (i == count - 1)
                {
                    Vector2 pointB = vertices[i];

                    Vector2 normalA = CalcNormalA();

                    Vector2 vertex1 = pointB + (normalA * width);
                    Vector2 vertex2 = pointB - (normalA * width);

                    AddPoint(vertex1);
                    AddPoint(vertex2);
                }
                else
                {
                    Vector2 pointB = vertices[i];

                    Vector2 normalA = CalcNormalA();
                    Vector2 normalB = CalcNormalB();

                    Vector2 cornerA = Vector2.Lerp(normalA, normalB, 0.5f).normalized;

                    Vector2 vertex1 = pointB + (cornerA * width / Vector2.Dot(cornerA, normalA));
                    Vector2 vertex2 = pointB - (cornerA * width / Vector2.Dot(cornerA, normalB));

                    AddPoint(vertex1);
                    AddPoint(vertex2);
                }
            }
            DrawCleanup();
        }
        private static void DrawThickPolylineClosed(float width, int actualNumberOfPoints, IReadOnlyList<Vector2> vertices)
        {
            if (vertices == null || vertices.Count < 2)
            {
                Debug.LogWarning("Polyline must have at least two points.");
                return;
            }

            DrawSetup(GL.TRIANGLE_STRIP, State.Stroke);
            var count = Mathf.Min(Mathf.Max(0, actualNumberOfPoints), vertices.Count);
            for (int i = 0; i <= count; i++)
            {
                var index = i % count;
                Vector2 CalcNormalA()
                {
                    Vector2 pointA = vertices[index == 0 ? (count - 1) : (index - 1)];
                    Vector2 pointB = vertices[index];

                    Vector2 directionA = (pointB - pointA).normalized;

                    return new Vector2(-directionA.y, directionA.x);
                }
                Vector2 CalcNormalB()
                {
                    Vector2 pointB = vertices[index];
                    Vector2 pointC = vertices[(index + 1) % count];

                    Vector2 directionB = (pointC - pointB).normalized;

                    return new Vector2(-directionB.y, directionB.x);
                }
                Vector2 pointB = vertices[index];

                Vector2 normalA = CalcNormalA();
                Vector2 normalB = CalcNormalB();

                Vector2 cornerA = Vector2.Lerp(normalA, normalB, 0.5f).normalized;

                Vector2 vertex1 = pointB + (cornerA * width / Vector2.Dot(cornerA, normalA));
                Vector2 vertex2 = pointB - (cornerA * width / Vector2.Dot(cornerA, normalB));

                AddPoint(vertex1);
                AddPoint(vertex2);
            }
            DrawCleanup();
        }
        private static void DrawThinPolyline(int actualNumberOfPoints, IReadOnlyList<Vector2> vertices)
        {
            if (vertices == null || vertices.Count < 2)
            {
                Debug.LogWarning("Polyline must have at least two points.");
                return;
            }

            DrawSetup(GL.LINES, State.Stroke);
            var count = Mathf.Min(Mathf.Max(0, actualNumberOfPoints), vertices.Count);
            for (int i = 0; i < count - 1; i++)
            {
                AddPoint(vertices[i]);
                AddPoint(vertices[i + 1]);
            }
            DrawCleanup();
        }
        private static void DrawThinPolylineClosed(int actualNumberOfPoints, IReadOnlyList<Vector2> vertices)
        {
            if (vertices == null || vertices.Count < 2)
            {
                Debug.LogWarning("Polyline must have at least two points.");
                return;
            }

            DrawSetup(GL.LINES, State.Stroke);
            var count = Mathf.Min(Mathf.Max(0, actualNumberOfPoints), vertices.Count);
            for (int i = 0; i < count; i++)
            {
                AddPoint(vertices[i]);
                AddPoint(vertices[(i + 1) % count]);
            }
            DrawCleanup();
        }
        private static void DrawPolyline(int actualNumberOfPoints, IReadOnlyList<Vector2> vertices, bool closed = false)
        {
            if (State.StrokeWeight <= 1)
            {
                if (closed)
                {
                    DrawThinPolylineClosed(actualNumberOfPoints, vertices);
                }
                else
                {
                    DrawThinPolyline(actualNumberOfPoints, vertices);
                }
            }
            else
            {
                if (closed)
                {
                    DrawThickPolylineClosed(State.StrokeWeight, actualNumberOfPoints, vertices);
                }
                else
                {
                    DrawThickPolyline(State.StrokeWeight, actualNumberOfPoints, vertices);
                }
            }
        }
        private static void DrawPolyline(IReadOnlyList<Vector2> vertices, bool closed = false)
        {
            DrawPolyline(vertices.Count, vertices, closed);
        }

        private static void AddPoint(Vector2 p1)
        {
            if (State.GradientFill != null)
            {
                var gf = State.GradientFill;
                var trn = Matrix4x4.Translate(gf.Center)
                    * Matrix4x4.Rotate(Quaternion.Euler(0, 0, gf.Rotation))
                    * Matrix4x4.Scale(new Vector3(gf.Scale.x, gf.Scale.y, 1))
                    * Matrix4x4.Scale(new Vector3(gf.Size.x / gf.Texture.width, gf.Size.y / gf.Texture.height, 1))
                    * Matrix4x4.Translate(new Vector3(-gf.Texture.width / 2f, -gf.Texture.height / 2f, 0))
                    ;
                var p2 = trn.inverse.MultiplyPoint(p1);
                var uv = p2 / new Vector2(gf.Texture.width, gf.Texture.height);
                GL.TexCoord2(uv.x, uv.y);
            }
            GL.Vertex(_oldMatrix.MultiplyPoint(p1));
        }

        private static Matrix4x4 _oldMatrix; // local to world matrix

        private static void DrawSetup(int drawMode, Color color)
        {
            _oldMatrix = GUI.matrix;
            // NOTE: Must be in world space for clipping
            // TODO: Might be a way to do this by inverting clipping rect. Test later.
            GUI.matrix = Matrix4x4.identity;

            GL.PushMatrix();
            UpdateClipRect();
            UpdateTexture();
            if (!Material.SetPass(0))
            {
                Debug.LogWarning("Failed to set material pass");
            }
            GL.Begin(drawMode);
            GL.Color(color);
        }
        private static void DrawCleanup()
        {
            GL.End();
            GL.PopMatrix();

            GUI.matrix = _oldMatrix;
        }
        private static void DrawFilledConvexPolygon(IReadOnlyList<Vector2> vertices)
        {
            if (vertices == null || vertices.Count < 3)
            {
                Debug.LogWarning("Polygon must have at least three points.");
                return;
            }

            DrawSetup(GL.TRIANGLES, State.Fill);
            for (int i = 1; i < vertices.Count - 1; i++)
            {
                AddPoint(vertices[0]);
                AddPoint(vertices[i]);
                AddPoint(vertices[i + 1]);
            }
            DrawCleanup();
        }
        private static void DrawFilledStar(Vector2 center, float innerRadius, float outerRadius, int numberOfPoints, float rotation)
        {
            DrawSetup(GL.TRIANGLES, State.Fill);
            float angleStep = Mathf.PI * 2 / numberOfPoints;
            float halfAngleStep = angleStep * 0.5f;

            for (int i = 0; i < numberOfPoints; i++)
            {
                float angle = angleStep * i + rotation;
                float nextAngle = angleStep * (i + 1) + rotation;

                Vector2 innerPoint1 = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * innerRadius;
                Vector2 outerPoint = center + new Vector2(Mathf.Cos(angle + halfAngleStep), Mathf.Sin(angle + halfAngleStep)) * outerRadius;
                Vector2 innerPoint2 = center + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * innerRadius;

                AddPoint(innerPoint1);
                AddPoint(outerPoint);
                AddPoint(center);
                AddPoint(outerPoint);
                AddPoint(innerPoint2);
                AddPoint(center);
            }
            DrawCleanup();
        }
        private static void DrawStarOutline(Vector2 center, float innerRadius, float outerRadius, int numberOfPoints, float rotation)
        {
            DrawSetup(GL.LINES, State.Stroke);
            float angleStep = Mathf.PI * 2 / numberOfPoints;
            float halfAngleStep = angleStep * 0.5f;

            for (int i = 0; i < numberOfPoints; i++)
            {
                float angle = angleStep * i + rotation;
                float nextAngle = angleStep * (i + 1) + rotation;

                Vector2 innerPoint1 = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * innerRadius;
                Vector2 outerPoint = center + new Vector2(Mathf.Cos(angle + halfAngleStep), Mathf.Sin(angle + halfAngleStep)) * outerRadius;
                Vector2 innerPoint2 = center + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * innerRadius;

                AddPoint(innerPoint1);
                AddPoint(outerPoint);
                AddPoint(outerPoint);
                AddPoint(innerPoint2);
            }
            DrawCleanup();
        }
        private static void DrawFilledVariableRadiusPolygon(Vector2 center, IReadOnlyList<float> radii, float rotation)
        {
            DrawSetup(GL.TRIANGLES, State.Fill);
            float angleStep = Mathf.PI * 2 / radii.Count;

            for (int i = 0; i < radii.Count; i++)
            {
                float angle = angleStep * i + rotation;
                float nextAngle = angleStep * (i + 1) + rotation;

                Vector2 currPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radii[i];
                Vector2 nextPoint = center + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radii[i + 1 % radii.Count];

                AddPoint(currPoint);
                AddPoint(nextPoint);
                AddPoint(center);
            }
            DrawCleanup();
        }
        private static void DrawVariableRadiusPolygonOutline(Vector2 center, IReadOnlyList<float> radii, float rotation)
        {
            DrawSetup(GL.LINES, State.Stroke);
            float angleStep = Mathf.PI * 2 / radii.Count;

            for (int i = 0; i < radii.Count; i++)
            {
                float angle = angleStep * i + rotation;
                float nextAngle = angleStep * (i + 1) + rotation;

                Vector2 currPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radii[i];
                Vector2 nextPoint = center + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radii[i + 1 % radii.Count];

                AddPoint(currPoint);
                AddPoint(nextPoint);
            }
            DrawCleanup();
        }

        private static Vector2[] GenerateEllipsePoints(Vector2 center, float width, float height, int segments)
        {
            var points = new Vector2[segments];
            float angle = 0;
            float increment = 2 * Mathf.PI / segments;

            for (int i = 0; i < segments; i++)
            {
                points[i] = new Vector2(
                    center.x + (width / 2 * Mathf.Cos(angle)),
                    center.y + (height / 2 * Mathf.Sin(angle))
                );
                angle += increment;
            }

            return points;
        }
        private static Vector2[] GenerateArcPoints(Vector2 center, float width, float height, float startAngle, float stopAngle, ArcDrawMode mode, int segments)
        {
            int arcSegments = Mathf.Max(2, Mathf.CeilToInt(segments * Mathf.Abs(stopAngle - startAngle) / (2 * Mathf.PI)));
            bool addCenter;
            {
                if (mode == ArcDrawMode.Pie || mode == ArcDrawMode.Center)
                {
                    addCenter = true;
                }
                else if (mode == ArcDrawMode.Chord || mode == ArcDrawMode.Open)
                {
                    addCenter = false;
                }
                else
                {
                    addCenter = true;
                    Debug.LogWarning($"Unknown arc draw mode '{mode}'");
                }
            }
            var points = new Vector2[arcSegments + 2 + (addCenter ? 1 : 0)];

            float angle = startAngle;
            float increment = (stopAngle - startAngle) / arcSegments;

            for (int i = 0; i <= arcSegments; i++)
            {
                points[i] = new Vector2(
                    center.x + (width / 2 * Mathf.Cos(angle)),
                    center.y + (height / 2 * Mathf.Sin(angle))
                );
                angle += increment;
            }

            if (addCenter)
            {
                points[arcSegments + 1] = center;
                points[arcSegments + 2] = points[0];
            }
            else
            {
                points[arcSegments + 1] = points[0];
            }

            return points;
        }

        private static Vector2[] GenerateRectanglePoints(Vector2 topLeft, float width, float height)
        {
            var points = new Vector2[4];

            points[0] = topLeft;
            points[1] = topLeft + new Vector2(width, 0);
            points[2] = topLeft + new Vector2(width, height);
            points[3] = topLeft + new Vector2(0, height);

            return points;
        }
        private static Vector2[] GenerateRoundedRectanglePoints(Vector2 bottomLeft, float width, float height, float tl, float tr, float br, float bl, int segments)
        {
            segments = Mathf.Max(1, segments);

            int numPoints = 4 * segments;
            Vector2[] points = new Vector2[numPoints];

            float cornerAngleIncrement = Mathf.PI / 2 / segments;

            int pointIndex = 0;

            // Generate points for each rounded corner
            for (int corner = 0; corner < 4; corner++)
            {
                float cornerRadius;
                Vector2 cornerCenter;
                float startAngle;

                switch (corner)
                {
                    case 0: // Top-left corner
                        cornerRadius = tl;
                        cornerCenter = bottomLeft + new Vector2(cornerRadius, cornerRadius);
                        startAngle = Mathf.PI;
                        break;
                    case 1: // Top-right corner
                        cornerRadius = tr;
                        cornerCenter = bottomLeft + new Vector2(width - cornerRadius, cornerRadius);
                        startAngle = 3 * Mathf.PI / 2;
                        break;
                    case 2: // Bottom-right corner
                        cornerRadius = br;
                        cornerCenter = bottomLeft + new Vector2(width - cornerRadius, height - cornerRadius);
                        startAngle = 0;
                        break;
                    default: // Bottom-left corner
                        cornerRadius = bl;
                        cornerCenter = bottomLeft + new Vector2(cornerRadius, height - cornerRadius);
                        startAngle = Mathf.PI / 2;
                        break;
                }

                for (int segment = 0; segment < segments; segment++)
                {
                    float angle = startAngle + segment * cornerAngleIncrement;
                    points[pointIndex++] = new Vector2(
                        cornerCenter.x + cornerRadius * Mathf.Cos(angle),
                        cornerCenter.y + cornerRadius * Mathf.Sin(angle)
                    );
                }
            }

            return points;
        }
        private static Vector2 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 point = uuu * p0;
            point += 3 * uu * t * p1;
            point += 3 * u * tt * p2;
            point += ttt * p3;

            return point;
        }
        private static Vector2[] GenerateCubicBezierPoints(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int numPoints)
        {
            Vector2[] points = new Vector2[numPoints];
            float t;

            for (int i = 0; i < numPoints; i++)
            {
                t = (float)i / (numPoints - 1);
                points[i] = CalculateCubicBezierPoint(t, p0, p1, p2, p3);
            }

            return points;
        }
        private static Vector2[] GenerateQuadraticBezierPoints(Vector2 p0, Vector2 p1, Vector2 p2, int numPoints)
        {
            Vector2[] points = new Vector2[numPoints];
            float t;

            for (int i = 0; i < numPoints; i++)
            {
                t = (float)i / (numPoints - 1);
                points[i] = CalculateQuadraticBezierPoint(t, p0, p1, p2);
            }

            return points;
        }

        private static Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector2 point = uu * p0;
            point += 2 * u * t * p1;
            point += tt * p2;

            return point;
        }

        private static List<Vector2> ApproximateCubicBSpline(IReadOnlyList<Vector2> controlPoints, int segmentsPerCurve = 10)
        {
            if (controlPoints == null || controlPoints.Count < 4)
            {
                Debug.LogError("At least 4 control points are required for a cubic B-spline.");
                return null;
            }

            List<Vector2> polylinePoints = new List<Vector2>();

            for (int i = 0; i < controlPoints.Count - 3; i++)
            {
                Vector2 p0 = controlPoints[i];
                Vector2 p1 = controlPoints[i + 1];
                Vector2 p2 = controlPoints[i + 2];
                Vector2 p3 = controlPoints[i + 3];

                polylinePoints.AddRange(BSpline(p0, p1, p2, p3, segmentsPerCurve));
            }

            return polylinePoints;
        }
        private static List<Vector2> BSpline(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, int divisions)
        {
            var res = new List<Vector2>();
            var a0 = (-p1.x + 3f * p2.x - 3f * p3.x + p4.x) / 6.0f;
            var a1 = (3f * p1.x - 6f * p2.x + 3f * p3.x) / 6.0f;
            var a2 = (-3f * p1.x + 3f * p3.x) / 6.0f;
            var a3 = (p1.x + 4f * p2.x + p3.x) / 6.0f;
            var b0 = (-p1.y + 3f * p2.y - 3f * p3.y + p4.y) / 6.0f;
            var b1 = (3f * p1.y - 6f * p2.y + 3f * p3.y) / 6.0f;
            var b2 = (-3f * p1.y + 3f * p3.y) / 6.0f;
            var b3 = (p1.y + 4f * p2.y + p3.y) / 6.0f;
            res.Add(new Vector2(a3, b3));
            for (int i = 1; i < divisions; i++)
            {
                float t;
                t = i / ((float)divisions);
                res.Add(new Vector2(
                            a3 + t * (a2 + t * (a1 + t * a0)),
                            b3 + t * (b2 + t * (b1 + t * b0))));
            }
            return res;
        }

        private static Vector2[] CalcArrowheadPoints(Vector2 start, Vector2 end, float widthRatio)
        {
            Vector2[] points = new Vector2[3];
            Vector2 direction = end - start;
            float distance = direction.magnitude;
            float width = distance * widthRatio;

            Vector2 normalizedDirection = direction.normalized;
            Vector2 perpendicular = new Vector2(-normalizedDirection.y, normalizedDirection.x);

            points[0] = start + perpendicular * (width * 0.5f);
            points[1] = start - perpendicular * (width * 0.5f);
            points[2] = end;

            return points;
        }

        #region Exposed
        internal static readonly IRenderState State = new RenderState();
        internal interface IRenderState
        {
            Color Fill { get; set; }
            Color Stroke { get; set; }
            IGradient GradientFill { get; set; }
            int StrokeWeight { get; set; }
        }
        internal static void EllipseImpl(Vector2 center, float width, float height, int segments)
        {
            var points = GenerateEllipsePoints(center, width, height, segments);
            if (State.Fill != Color.clear)
            {
                DrawFilledConvexPolygon(points);
            }
            if (State.Stroke != Color.clear)
            {
                DrawPolyline(points);
            }
        }
        internal static void ArcImpl(Vector2 center, float width, float height, float startAngle, float stopAngle, ArcDrawMode mode, int segments)
        {
            var points = GenerateArcPoints(center, width, height, startAngle, stopAngle, mode, segments);
            if (State.Fill != Color.clear)
            {
                if ((mode == ArcDrawMode.Pie || mode == ArcDrawMode.Center) && (stopAngle - startAngle > Mathf.PI))
                {
                    // Must make convex
                    var count = Mathf.CeilToInt(((points.Length - 2) / 2f) + 0.5f);
                    var centerPoint = points[points.Length - 2];
                    var points1 = new Vector2[count + 2];
                    var points2 = new Vector2[count + 2];
                    for (int i = 0; i < count + 1; ++i)
                    {
                        points1[i] = points[i];
                    }
                    points1[points1.Length - 1] = centerPoint;
                    for (int i = 0; i < count + 1; ++i)
                    {
                        points2[i] = points[i + count];
                    }
                    points2[points2.Length - 1] = centerPoint;
                    DrawFilledConvexPolygon(points1);
                    DrawFilledConvexPolygon(points2); // TODO: Optimize
                }
                else
                {
                    DrawFilledConvexPolygon(points);
                }
                // DrawFilledConvexPolygon(points);
            }
            if (State.Stroke != Color.clear)
            {
                int numberOfPoints;
                {
                    if (mode == ArcDrawMode.Center)
                    {
                        numberOfPoints = points.Length - 2;
                    }
                    else if (mode == ArcDrawMode.Open)
                    {
                        numberOfPoints = points.Length - 1;
                    }
                    else
                    {
                        numberOfPoints = points.Length;
                    }
                }
                DrawPolyline(numberOfPoints, points);
            }
        }

        internal static void RectImp(Vector2 bottomLeft, float width, float height, float tl, float tr, float br, float bl, int segments)
        {
            Vector2[] points;
            if (tl == 0 && tr == 0 && br == 0 && bl == 0)
            {
                points = GenerateRectanglePoints(bottomLeft, width, height);
            }
            else
            {
                points = GenerateRoundedRectanglePoints(bottomLeft, width, height, tl, tr, br, bl, segments);
            }

            if (State.Fill != Color.clear)
            {
                DrawFilledConvexPolygon(points);
            }

            if (State.Stroke != Color.clear)
            {
                DrawPolyline(points.Length, points, closed: true);
            }
        }

        internal static void LineImpl(Vector2 start, Vector2 end)
        {
            var points = new[] { start, end };
            if (State.Stroke != Color.clear)
            {
                DrawPolyline(points);
            }
        }
        internal static void StarImp(float x, float y, float innerRadius, float outerRadius, int points, float rotation)
        {
            if (State.Fill != Color.clear)
            {
                DrawFilledStar(new Vector2(x, y), innerRadius, outerRadius, points, rotation);
            }

            if (State.Stroke != Color.clear)
            {
                DrawStarOutline(new Vector2(x, y), innerRadius, outerRadius, points, rotation);
            }
        }
        internal static void VariableRadiusPolygonImpl(float x, float y, IReadOnlyList<float> radii, float rotation)
        {
            if (State.Fill != Color.clear)
            {
                DrawFilledVariableRadiusPolygon(new Vector2(x, y), radii, rotation);
            }

            if (State.Stroke != Color.clear)
            {
                DrawVariableRadiusPolygonOutline(new Vector2(x, y), radii, rotation);
            }
        }
        internal static void CubicBezierImpl(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int numPoints)
        {
            if (State.Stroke != Color.clear)
            {
                var points = GenerateCubicBezierPoints(p0, p1, p2, p3, numPoints);
                DrawPolyline(points.Length, points);
            }
        }
        internal static void QuadraticBezierImpl(Vector2 p0, Vector2 p1, Vector2 p2, int numPoints)
        {
            if (State.Stroke != Color.clear)
            {
                var points = GenerateQuadraticBezierPoints(p0, p1, p2, numPoints);
                DrawPolyline(points.Length, points);
            }
        }
        internal static void BSplineImpl(IReadOnlyList<Vector2> controlPoints, int segmentsPerCurve)
        {
            if (controlPoints == null || controlPoints.Count < 4)
            {
                return;
            }

            var points = ApproximateCubicBSpline(controlPoints, segmentsPerCurve);

            DrawPolyline(points);
        }
        internal static void ArrowheadImpl(Vector2 start, Vector2 end, float widthRatio)
        {
            var points = CalcArrowheadPoints(start, end, widthRatio);
            if (State.Fill != Color.clear)
            {
                DrawFilledConvexPolygon(points);
            }

            if (State.Stroke != Color.clear)
            {
                DrawPolyline(points, closed: true);
            }
        }
        #endregion Exposed
    }
}
