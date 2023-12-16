using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Levers
{
    internal static class DrawImplementations
    {
        private static readonly Stack<IRenderState> _stateStack = new Stack<IRenderState>();
        private static readonly IRenderState _defaultState = new RenderState();
        private static void UpdateTexture(Material material)
        {
            if (State.TextureFill == null)
            {
                material.SetTexture("_MainTex", null);
            }
            else
            {
                material.SetTexture("_MainTex", State.TextureFill.Texture);
            }
        }
        private class RenderState : IRenderState
        {
            public Color Fill { get; set; } = Color.white;
            public Color Stroke { get; set; } = Color.clear;
            public ITextureFill TextureFill { get; set; } = null;
            public int StrokeWeight { get; set; } = 1;
            public float CurvePrecision { get; set; } = 0.5f;
            public bool AlwaysDraw { get; set; } = false;

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
        private static void UpdateClipRect(Material material)
        {
            Rect? visibleRect = GetVisibleRect();
            if (visibleRect != null)
            {
                var vr = visibleRect.Value;
                var vect = new Vector4(vr.xMin, vr.yMin, vr.xMax, vr.yMax);
                material.SetVector(_clipRectVariableName, vect);
                if (!material.IsKeywordEnabled(_unityUiClipRectKeyword))
                {
                    material.EnableKeyword(_unityUiClipRectKeyword);
                }
            }
            else
            {
                material.SetVector(_clipRectVariableName, new Vector4(0, 0, Screen.width, Screen.height));
                if (material.IsKeywordEnabled(_unityUiClipRectKeyword))
                {
                    material.DisableKeyword(_unityUiClipRectKeyword);
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
        private static Material __fillComplexPolygonMaterial;
        private static Material FillComplexPolygonMaterial
        {
            get
            {
                if (__fillComplexPolygonMaterial == null)
                {
                    var shader = Shader.Find("Hidden/Com/Amequus/Levers/FillComplexPolygon");
                    __fillComplexPolygonMaterial = new Material(shader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }
                return __fillComplexPolygonMaterial;
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
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
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
            if (State.TextureFill != null)
            {
                var gf = State.TextureFill;
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
            UpdateClipRect(Material);
            UpdateTexture(Material);
            if (!Material.SetPass(0))
            {
                Debug.LogWarning("Failed to set material pass");
            }
            GL.Begin(drawMode);
            GL.Color(color);
        }
        internal const int MaxPolygonVertices = 512;
        private static readonly Vector4[] _polygonVertices = new Vector4[MaxPolygonVertices];
        private static void DrawSetupComplexPolygon(Path2D.Partition partition, Color color)
        {
            _oldMatrix = GUI.matrix;
            // NOTE: Must be in world space for clipping
            // TODO: Might be a way to do this by inverting clipping rect. Test later.
            GUI.matrix = Matrix4x4.identity;

            GL.PushMatrix();
            UpdateClipRect(FillComplexPolygonMaterial);
            UpdateTexture(FillComplexPolygonMaterial);

            { // NOTE: Can probably move this section out to unify with DrawSetup
                if (partition.Edges.Count * 2 > MaxPolygonVertices)
                {
                    throw new Exception("Too many vertices in polygon: " + partition.Edges.Count);
                }
                for (int i = 0; i < partition.Edges.Count; ++i)
                {
                    var vertexA = partition.Edges[i].Item1;
                    var vertexB = partition.Edges[i].Item2;
                    _polygonVertices[i * 2] = (Vector4)_oldMatrix.MultiplyPoint(vertexA);
                    _polygonVertices[(i * 2) + 1] = (Vector4)_oldMatrix.MultiplyPoint(vertexB);
                }
                // Debug.Log("Polygon vertices: " + string.Join(", ", _polygonVertices.Select(v => v.ToString()).ToArray()));
                FillComplexPolygonMaterial.SetVectorArray("_polygonVertices", _polygonVertices);
                FillComplexPolygonMaterial.SetInt("_vertexCount", partition.Edges.Count * 2);
            }
            if (!FillComplexPolygonMaterial.SetPass(0))
            {
                Debug.LogWarning("Failed to set polygon material pass");
            }
            GL.Begin(GL.TRIANGLES);
            GL.Color(color);
        }
        private static void DrawCleanup()
        {
            GL.End();
            GL.PopMatrix();

            GUI.matrix = _oldMatrix;
        }
        internal static void DrawFilledComplexPolygon(Path2D path)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            // NOTE: Need to test with rotation and scale matrix, make sure doesn't clip
            var partitions = path.Partitions;
            foreach (var partition in partitions)
            {
                var minX = path.Bounds.min.x;
                var minY = partition.Start;
                var maxX = path.Bounds.max.x;
                var maxY = partition.End;
                DrawSetupComplexPolygon(partition, State.Fill); // NOTE: Might be able to change this to GL.QUADS
                AddPoint(new Vector2(minX, minY));
                AddPoint(new Vector2(maxX, minY));
                AddPoint(new Vector2(maxX, maxY));
                AddPoint(new Vector2(minX, minY));
                AddPoint(new Vector2(maxX, maxY));
                AddPoint(new Vector2(minX, maxY));
                DrawCleanup();
            }
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
        internal static void PushStateImpl()
        {
            _stateStack.Push(new RenderState());
        }
        internal static void PopStateImpl()
        {
            if (_stateStack.Count == 0)
            {
                Debug.LogWarning("Render state stack is empty");
                return;
            }
            _stateStack.Pop();
        }
        internal static IRenderState State => _stateStack.Count > 0 ? _stateStack.Peek() : _defaultState;
        internal interface IRenderState
        {
            Color Fill { get; set; }
            Color Stroke { get; set; }
            ITextureFill TextureFill { get; set; }
            int StrokeWeight { get; set; }
            float CurvePrecision { get; set; }
            /// <summary>
            /// Should drawing be done outside EventType.Repaint?
            /// </summary>
            bool AlwaysDraw { get; set; }
        }
        internal static void EllipseImpl(Vector2 center, float width, float height, int segments)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
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
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
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
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
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
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            var points = new[] { start, end };
            if (State.Stroke != Color.clear)
            {
                DrawPolyline(points);
            }
        }
        internal static void StarImp(float x, float y, float innerRadius, float outerRadius, int points, float rotation)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
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
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (State.Fill != Color.clear)
            {
                DrawFilledVariableRadiusPolygon(new Vector2(x, y), radii, rotation);
            }

            if (State.Stroke != Color.clear)
            {
                DrawVariableRadiusPolygonOutline(new Vector2(x, y), radii, rotation);
            }
        }
        internal static void CubicBezierImpl(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (State.Stroke != Color.clear)
            {
                var points = PathComputation.GenerateCubicBezierPoints(p0, p1, p2, p3, State.CurvePrecision);
                DrawPolyline(points.Length, points);
            }
        }
        internal static void QuadraticBezierImpl(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (State.Stroke != Color.clear)
            {
                var points = PathComputation.GenerateQuadraticBezierPoints(p0, p1, p2, State.CurvePrecision);
                DrawPolyline(points.Length, points);
            }
        }
        internal static void BSplineImpl(IReadOnlyList<Vector2> controlPoints, int segmentsPerCurve)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (controlPoints == null || controlPoints.Count < 4)
            {
                return;
            }

            var points = PathComputation.ApproximateCubicBSpline(controlPoints, segmentsPerCurve);

            DrawPolyline(points);
        }
        internal static void ArrowheadImpl(Vector2 start, Vector2 end, float widthRatio)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
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
        internal static void DrawPathImpl(Path2D path)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            var points = path.ExportPoints();
            if (State.Fill != Color.clear)
            {
                DrawFilledComplexPolygon(path);
            }

            if (State.Stroke != Color.clear)
            {
                DrawPolyline(points, closed: false);
            }
        }
        #endregion Exposed
    }
}
