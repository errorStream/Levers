using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Levers
{
    internal static class DrawImplementations
    {
        private static readonly Stack<IRenderState> _stateStack = new Stack<IRenderState>();
        private static readonly IRenderState _defaultState = new RenderState();
        private class RenderState : IRenderState
        {
            public Color Fill { get; set; } = Color.white;
            public Color Stroke { get; set; } = Color.clear;
            public ITextureFill TextureFill { get; set; } = null;
            public float StrokeWeight { get; set; } = 1;
            public float CurvePrecision { get; set; } = 0.5f;
            public bool AlwaysDraw { get; set; } = false;
            public float AntiAliasing { get; set; } = 1;

            internal RenderState()
            {

            }
        }
        private const string _unityUiClipRectKeyword = "UNITY_UI_CLIP_RECT";
        private const string _clipRectVariableName = "_ClipRect";
        private static Func<Rect> _visibleRectGetter;
        /// <summary>
        /// Retrives the internal value of <c>UnityEngine.GUIClip.visibleRect</c> if it exists.
        /// </summary>
        private static Rect? GetVisibleRect()
        {
            if (_visibleRectGetter == null)
            {
                System.Type guiClipType = typeof(GUI).Assembly.GetType("UnityEngine.GUIClip");
                if (guiClipType != null)
                {
                    var prop = guiClipType.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    _visibleRectGetter = prop == null ? null : (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), prop.GetGetMethod(true));
                }
            }
            var res = _visibleRectGetter == null ? null : (Rect?)_visibleRectGetter();
            return res;
        }
        /// <summary>
        /// Updates <paramref name="material"/> with the current value of the Unity ui clip rect if it exists.
        /// </summary>
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

        private static void AddPoint(Vector2 p1, Vector2? uv2 = null)
        {
            var uv = Vector2.zero;
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
                uv = p2 / new Vector2(gf.Texture.width, gf.Texture.height);
            }
            GL.MultiTexCoord(0, uv);
            if (uv2.HasValue)
            {
                GL.MultiTexCoord(1, uv2.Value);
            }
            else
            {
                GL.MultiTexCoord(1, new Vector2(1000, 1000));
            }
            GL.Vertex(_oldMatrix.MultiplyPoint(p1));
        }

        private static Matrix4x4 _oldMatrix; // local to world matrix

        internal const int MaxPolygonVertices = 512;
        private static readonly Vector4[] _polygonVertices = new Vector4[MaxPolygonVertices];
        private static void DrawSetupComplexPolygon(Path2D.IPartition partition, Color color)
        {
            _oldMatrix = GUI.matrix;
            // NOTE: Must be in world space for clipping
            // TODO: Might be a way to do this by inverting clipping rect. Test later.
            GUI.matrix = Matrix4x4.identity;

            GL.PushMatrix();
            UpdateClipRect(FillComplexPolygonMaterial);
            FillComplexPolygonMaterial.SetTexture("_MainTex", State.TextureFill?.Texture);
            FillComplexPolygonMaterial.SetColor("_StrokeColor", State.Stroke);
            FillComplexPolygonMaterial.SetFloat("_StrokeWeight", State.StrokeWeight);
            FillComplexPolygonMaterial.SetFloat("_AntiAliasing", State.AntiAliasing);

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
            if (path.Bounds == null)
            {
                return;
            }
            Rect bounds = path.Bounds.Value;
            // NOTE: Need to test with rotation and scale matrix, make sure doesn't clip
            var partitions = path.Partitions;
            for (int i = 0; i < partitions.Count; ++i)
            {
                var partition = partitions[i];
                float buffer = State.StrokeWeight + State.AntiAliasing;
                var minX = bounds.min.x - buffer;
                var minY = partition.Start;
                if (Mathf.Approximately(partition.Start, bounds.min.y)) { minY -= buffer; }
                var maxX = bounds.max.x + buffer;
                var maxY = partition.End;
                if (Mathf.Approximately(partition.End, bounds.max.y)) { maxY += buffer; }
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
        private static void CalcVariableRadiusPolygon(Vector2 center, IReadOnlyList<float> radii, float rotation, Action<Vector2> onPoint)
        {
            float angleStep = Mathf.PI * 2 / radii.Count;

            for (int i = 0; i < radii.Count; i++)
            {
                float angle = angleStep * i + rotation;
                float nextAngle = angleStep * (i + 1) + rotation;

                Vector2 currPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radii[i];
                Vector2 nextPoint = center + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radii[(i + 1) % radii.Count];

                onPoint(currPoint);
                onPoint(nextPoint);
            }
        }

        private static void GenerateEllipsePoints(Vector2 center, float width, float height, int segments, Action<Vector2> onPoint)
        {
            // var points = new Vector2[segments];
            float angle = 0;
            float increment = 2 * Mathf.PI / segments;

            for (int i = 0; i < segments; i++)
            {
                onPoint(new Vector2(
                    center.x + (width / 2 * Mathf.Cos(angle)),
                    center.y + (height / 2 * Mathf.Sin(angle))
                ));
                angle += increment;
            }
        }

        private static void GenerateRectanglePoints(Vector2 topLeft, float width, float height, Action<Vector2> onPoint)
        {
            onPoint(topLeft);
            onPoint(topLeft + new Vector2(width, 0));
            onPoint(topLeft + new Vector2(width, height));
            onPoint(topLeft + new Vector2(0, height));
        }
        private static void GenerateRoundedRectanglePoints(Vector2 bottomLeft, float width, float height, float tl, float tr, float br, float bl, int segments, Action<Vector2> onPoint)
        {
            segments = Mathf.Max(1, segments);

            float cornerAngleIncrement = Mathf.PI / 2 / segments;

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
                    float angle = startAngle + (segment * cornerAngleIncrement);
                    onPoint(new Vector2(
                        cornerCenter.x + (cornerRadius * Mathf.Cos(angle)),
                        cornerCenter.y + (cornerRadius * Mathf.Sin(angle))
                    ));
                }
            }
        }
        private static void CalcArrowheadPoints(Vector2 start, Vector2 end, float widthRatio, Action<Vector2> onPoint)
        {
            Vector2 direction = end - start;
            float distance = direction.magnitude;
            float width = distance * widthRatio;

            Vector2 normalizedDirection = direction.normalized;
            Vector2 perpendicular = new Vector2(-normalizedDirection.y, normalizedDirection.x);

            onPoint(start + perpendicular * (width * 0.5f));
            onPoint(start - perpendicular * (width * 0.5f));
            onPoint(end);
        }
        private static void CalcStarPoints(Vector2 center, float innerRadius, float outerRadius, int numberOfPoints, float rotation, Action<Vector2> onPoint)
        {
            float angleStep = Mathf.PI * 2 / numberOfPoints;
            float halfAngleStep = angleStep * 0.5f;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float angle = (angleStep * i) + rotation;

                Vector2 innerPoint = center + (new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * innerRadius);
                Vector2 outerPoint = center + (new Vector2(Mathf.Cos(angle + halfAngleStep), Mathf.Sin(angle + halfAngleStep)) * outerRadius);

                onPoint(innerPoint);
                onPoint(outerPoint);
            }
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
            float StrokeWeight { get; set; }
            float CurvePrecision { get; set; }
            float AntiAliasing { get; set; }
            /// <summary>
            /// Should drawing be done outside EventType.Repaint?
            /// </summary>
            bool AlwaysDraw { get; set; }
        }
        private static readonly Path2D _path2D = new Path2D();
        internal static void EllipseImpl(Vector2 center, float width, float height, int segments)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            _path2D.Clear();
            GenerateEllipsePoints(center, width, height, segments, _path2D.LineToAction);
            _path2D.Close();
            DrawFilledComplexPolygon(_path2D);
        }
        internal static void ArcImpl(Vector2 center, float width, float height, float startAngle, float stopAngle, ArcDrawMode mode, int segments)
        {
            ArcImpl2(center, width, height, startAngle, stopAngle, mode, segments);
        }
        private static readonly List<Vector2> _arcPoints = new List<Vector2>();
        internal static void ArcImpl2(Vector2 center, float width, float height, float startAngle, float stopAngle, ArcDrawMode mode, int segments)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }

            bool noOutlineOnCut = (mode == ArcDrawMode.Open) || (mode == ArcDrawMode.Center);
            bool goesThroughCenter = (mode == ArcDrawMode.Center) || (mode == ArcDrawMode.Pie);
            {
                _path2D.Clear();

                if (noOutlineOnCut)
                {
                    _arcPoints.Clear();
                }
                if (goesThroughCenter)
                {
                    _path2D.LineTo(center);
                }
                int arcSegments = Mathf.Max(2, Mathf.CeilToInt(segments * Mathf.Abs(stopAngle - startAngle) / (2 * Mathf.PI)));
                float angle = startAngle;
                float increment = (stopAngle - startAngle) / arcSegments;
                for (int i = 0; i <= arcSegments; i++)
                {
                    var pnt = new Vector2(
                        center.x + (width / 2 * Mathf.Cos(angle)),
                        center.y + (height / 2 * Mathf.Sin(angle))
                    );
                    _path2D.LineTo(pnt);
                    if (noOutlineOnCut)
                    {
                        _arcPoints.Add(pnt);
                    }
                    angle += increment;
                }
                _path2D.Close();
                Color oldStroke = default;
                if (noOutlineOnCut)
                {
                    oldStroke = State.Stroke;
                    State.Stroke = Color.clear;
                }
                DrawFilledComplexPolygon(_path2D);
                if (noOutlineOnCut)
                {
                    State.Stroke = oldStroke;
                }
            }
            if (noOutlineOnCut)
            {
                _path2D.Clear();
                for (int i = 0; i < _arcPoints.Count; i++)
                {
                    _path2D.LineTo(_arcPoints[i]);
                }
                var oldFill = State.Fill;
                State.Fill = Color.clear;
                DrawFilledComplexPolygon(_path2D);
                State.Fill = oldFill;
            }
        }

        internal static void RectImp(Vector2 bottomLeft, float width, float height, float tl, float tr, float br, float bl, int segments)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            _path2D.Clear();

            if (tl == 0 && tr == 0 && br == 0 && bl == 0)
            {
                GenerateRectanglePoints(bottomLeft, width, height, _path2D.LineToAction);
            }
            else
            {
                GenerateRoundedRectanglePoints(bottomLeft, width, height, tl, tr, br, bl, segments, _path2D.LineToAction);
            }

            _path2D.Close();

            DrawFilledComplexPolygon(_path2D);
        }

        internal static void LineImpl(Vector2 start, Vector2 end)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            _path2D.Clear();
            _path2D.LineTo(start);
            _path2D.LineTo(end);
            var oldFill = State.Fill;
            State.Fill = Color.clear;
            DrawFilledComplexPolygon(_path2D);
            State.Fill = oldFill;
        }
        internal static void StarImp(float x, float y, float innerRadius, float outerRadius, int points, float rotation)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            _path2D.Clear();
            CalcStarPoints(new Vector2(x, y), innerRadius, outerRadius, points, rotation, _path2D.LineToAction);
            _path2D.Close();
            DrawFilledComplexPolygon(_path2D);
        }
        internal static void VariableRadiusPolygonImpl(float x, float y, IReadOnlyList<float> radii, float rotation)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            _path2D.Clear();
            CalcVariableRadiusPolygon(new Vector2(x, y), radii, rotation, _path2D.LineToAction);
            _path2D.Close();
            DrawFilledComplexPolygon(_path2D);
        }
        internal static void CubicBezierImpl(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            _path2D.Clear();
            _path2D.LineTo(p0);
            _path2D.BezierCurveTo(p1, p2, p3);
            var oldFill = State.Fill;
            State.Fill = Color.clear;
            DrawFilledComplexPolygon(_path2D);
            State.Fill = oldFill;
        }
        internal static void QuadraticBezierImpl(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            _path2D.Clear();
            _path2D.LineTo(p0);
            _path2D.QuadraticCurveTo(p1, p2);
            var oldFill = State.Fill;
            State.Fill = Color.clear;
            DrawFilledComplexPolygon(_path2D);
            State.Fill = oldFill;
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

            _path2D.Clear();
            PathComputation.ApproximateCubicBSpline(controlPoints, _path2D.LineToAction, segmentsPerCurve);
            DrawFilledComplexPolygon(_path2D);
        }
        internal static void ArrowheadImpl(Vector2 start, Vector2 end, float widthRatio)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            _path2D.Clear();
            CalcArrowheadPoints(start, end, widthRatio, _path2D.LineToAction);
            _path2D.Close();
            DrawFilledComplexPolygon(_path2D);
        }
        internal static void DrawPathImpl(Path2D path)
        {
            if (!State.AlwaysDraw && Event.current.type != EventType.Repaint)
            {
                return;
            }
            DrawFilledComplexPolygon(path);
        }
        #endregion Exposed
    }
}
