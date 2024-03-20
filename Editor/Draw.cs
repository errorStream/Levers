using System.Collections.Generic;
using UnityEngine;

namespace Levers
{
    public static class Draw
    {
        public static ITextureFill TextureFill
        {
            get => DrawImplementations.State.TextureFill;
            set => DrawImplementations.State.TextureFill = value;
        }
        public static Color Fill
        {
            get => DrawImplementations.State.Fill;
            set => DrawImplementations.State.Fill = value;
        }
        public static Color Stroke
        {
            get => DrawImplementations.State.Stroke;
            set => DrawImplementations.State.Stroke = value;
        }
        public static float StrokeWeight
        {
            get => DrawImplementations.State.StrokeWeight;
            set => DrawImplementations.State.StrokeWeight = value;
        }
        public static float AntiAliasing
        {
            get => DrawImplementations.State.AntiAliasing;
            set => DrawImplementations.State.AntiAliasing = value;
        }

        public static void Line(float x1, float y1, float x2, float y2)
        {
            DrawImplementations.LineImpl(new Vector2(x1, y1), new Vector2(x2, y2));
        }
        public static void Line(Vector2 start, float x2, float y2)
        {
            DrawImplementations.LineImpl(start, new Vector2(x2, y2));
        }
        public static void Line(float x1, float y1, Vector2 end)
        {
            DrawImplementations.LineImpl(new Vector2(x1, y1), end);
        }
        public static void Line(Vector2 start, Vector2 end)
        {
            DrawImplementations.LineImpl(start, end);
        }

        public static void Circle(float x, float y, float diameter)
        {
            DrawImplementations.EllipseImpl(new Vector2(x, y), diameter, diameter, 36);
        }
        public static void Circle(Vector2 center, float diameter)
        {
            DrawImplementations.EllipseImpl(center, diameter, diameter, 36);
        }

        public static void Ellipse(float x, float y, float width, float? height, int segments = 36)
        {
            DrawImplementations.EllipseImpl(new Vector2(x, y), width, height ?? width, segments);
        }
        public static void Ellipse(Vector2 center, float width, float? height, int segments = 36)
        {
            DrawImplementations.EllipseImpl(center, width, height ?? width, segments);
        }
        public static void Ellipse(Vector2 center, Vector2 size, int segments = 36)
        {
            DrawImplementations.EllipseImpl(center, size.x, size.y, segments);
        }
        public static void Ellipse(float x, float y, Vector2 size, int segments = 36)
        {
            DrawImplementations.EllipseImpl(new Vector2(x, y), size.x, size.y, segments);
        }
        public static void Ellipse(Rect bounds, int segments = 36)
        {
            DrawImplementations.EllipseImpl(new Vector2(bounds.x, bounds.y), bounds.width, bounds.height, segments);
        }

        public static void Arc(float x, float y, float w, float h, float start, float stop, ArcDrawMode mode = default, int detail = 25)
        {
            DrawImplementations.ArcImpl(new Vector2(x, y), w, h, start, stop, mode, detail);
        }
        public static void Arc(Vector2 center, float w, float h, float start, float stop, ArcDrawMode mode = default, int detail = 25)
        {
            DrawImplementations.ArcImpl(center, w, h, start, stop, mode, detail);
        }
        public static void Arc(Vector2 center, Vector2 size, float start, float stop, ArcDrawMode mode = default, int detail = 25)
        {
            DrawImplementations.ArcImpl(center, size.x, size.y, start, stop, mode, detail);
        }
        public static void Arc(float x, float y, Vector2 size, float start, float stop, ArcDrawMode mode = default, int detail = 25)
        {
            DrawImplementations.ArcImpl(new Vector2(x, y), size.x, size.y, start, stop, mode, detail);
        }
        public static void Arc(Rect bounds, float start, float stop, ArcDrawMode mode = default, int detail = 25)
        {
            DrawImplementations.ArcImpl(new Vector2(bounds.x, bounds.y), bounds.width, bounds.height, start, stop, mode, detail);
        }

        private const int DEFAULT_RECT_DETAIL = 8;
        public static void Rect(float x, float y, float w, float? h = null, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(x, y), w, h ?? w, tl, tr, br, bl, detail);
        }
        public static void Rect(Vector2 position, float w, float? h = null, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(position, w, h ?? w, tl, tr, br, bl, detail);
        }
        public static void Rect(float x, float y, Vector2 size, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(x, y), size.x, size.y, tl, tr, br, bl, detail);
        }
        public static void Rect(Vector2 position, Vector2 size, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(position, size.x, size.y, tl, tr, br, bl, detail);
        }
        public static void Rect(Rect bounds, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(bounds.x, bounds.y), bounds.width, bounds.height, tl, tr, br, bl, detail);
        }

        public static void Rect(float x, float y, float w, float h, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(x, y), w, h, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }
        public static void Rect(Vector2 position, float w, float h, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(position, w, h, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }
        public static void Rect(float x, float y, Vector2 size, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(x, y), size.x, size.y, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }
        public static void Rect(Vector2 position, Vector2 size, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(position, size.x, size.y, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }
        public static void Rect(Rect bounds, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(bounds.x, bounds.y), bounds.width, bounds.height, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }

        public static void Star(float x, float y, float innerRadius, float outerRadius, int points, float rotation = 0f)
        {
            DrawImplementations.StarImp(x, y, innerRadius, outerRadius, points, rotation);
        }
        public static void Star(Vector2 center, float innerRadius, float outerRadius, int points, float rotation = 0f)
        {
            DrawImplementations.StarImp(center.x, center.y, innerRadius, outerRadius, points, rotation);
        }
        public static void Star(float x, float y, IReadOnlyList<float> radii, float rotation = 0f)
        {
            DrawImplementations.VariableRadiusPolygonImpl(x, y, radii, rotation);
        }
        public static void Star(Vector2 center, IReadOnlyList<float> radii, float rotation = 0f)
        {
            DrawImplementations.VariableRadiusPolygonImpl(center.x, center.y, radii, rotation);
        }
        public static void Bezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x3, y3), new Vector2(x4, y4));
        }
        public static void Bezier(Vector2 p1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            DrawImplementations.CubicBezierImpl(p1, new Vector2(x2, y2), new Vector2(x3, y3), new Vector2(x4, y4));
        }
        public static void Bezier(float x1, float y1, Vector2 p2, float x3, float y3, float x4, float y4)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), p2, new Vector2(x3, y3), new Vector2(x4, y4));
        }
        public static void Bezier(float x1, float y1, float x2, float y2, Vector2 p3, float x4, float y4)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), new Vector2(x2, y2), p3, new Vector2(x4, y4));
        }
        public static void Bezier(float x1, float y1, float x2, float y2, float x3, float y3, Vector2 p4)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x3, y3), p4);
        }
        public static void Bezier(Vector2 p1, Vector2 p2, float x3, float y3, float x4, float y4)
        {
            DrawImplementations.CubicBezierImpl(p1, p2, new Vector2(x3, y3), new Vector2(x4, y4));
        }
        public static void Bezier(Vector2 p1, float x2, float y2, Vector2 p3, float x4, float y4)
        {
            DrawImplementations.CubicBezierImpl(p1, new Vector2(x2, y2), p3, new Vector2(x4, y4));
        }
        public static void Bezier(Vector2 p1, float x2, float y2, float x3, float y3, Vector2 p4)
        {
            DrawImplementations.CubicBezierImpl(p1, new Vector2(x2, y2), new Vector2(x3, y3), p4);
        }
        public static void Bezier(float x1, float y1, Vector2 p2, Vector2 p3, float x4, float y4)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), p2, p3, new Vector2(x4, y4));
        }
        public static void Bezier(float x1, float y1, Vector2 p2, float x3, float y3, Vector2 p4)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), p2, new Vector2(x3, y3), p4);
        }
        public static void Bezier(float x1, float y1, float x2, float y2, Vector2 p3, Vector2 p4)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), new Vector2(x2, y2), p3, p4);
        }
        public static void Bezier(Vector2 p1, Vector2 p2, Vector2 p3, float x4, float y4)
        {
            DrawImplementations.CubicBezierImpl(p1, p2, p3, new Vector2(x4, y4));
        }
        public static void Bezier(Vector2 p1, Vector2 p2, float x3, float y3, Vector2 p4)
        {
            DrawImplementations.CubicBezierImpl(p1, p2, new Vector2(x3, y3), p4);
        }
        public static void Bezier(Vector2 p1, float x2, float y2, Vector2 p3, Vector2 p4)
        {
            DrawImplementations.CubicBezierImpl(p1, new Vector2(x2, y2), p3, p4);
        }
        public static void Bezier(float x1, float y1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), p2, p3, p4);
        }
        public static void Bezier(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            DrawImplementations.CubicBezierImpl(p1, p2, p3, p4);
        }
        public static void Bezier(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            DrawImplementations.QuadraticBezierImpl(new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x3, y3));
        }
        public static void Bezier(Vector2 p1, float x2, float y2, float x3, float y3)
        {
            DrawImplementations.QuadraticBezierImpl(p1, new Vector2(x2, y2), new Vector2(x3, y3));
        }
        public static void Bezier(float x1, float y1, Vector2 p2, float x3, float y3)
        {
            DrawImplementations.QuadraticBezierImpl(new Vector2(x1, y1), p2, new Vector2(x3, y3));
        }
        public static void Bezier(float x1, float y1, float x2, float y2, Vector2 p3)
        {
            DrawImplementations.QuadraticBezierImpl(new Vector2(x1, y1), new Vector2(x2, y2), p3);
        }
        public static void Bezier(Vector2 p1, Vector2 p2, float x3, float y3)
        {
            DrawImplementations.QuadraticBezierImpl(p1, p2, new Vector2(x3, y3));
        }
        public static void Bezier(Vector2 p1, float x2, float y2, Vector2 p3)
        {
            DrawImplementations.QuadraticBezierImpl(p1, new Vector2(x2, y2), p3);
        }
        public static void Bezier(float x1, float y1, Vector2 p2, Vector2 p3)
        {
            DrawImplementations.QuadraticBezierImpl(new Vector2(x1, y1), p2, p3);
        }
        public static void Bezier(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            DrawImplementations.QuadraticBezierImpl(p1, p2, p3);
        }
        public static void BSpline(IReadOnlyList<Vector2> controlPoints, int detail = 20)
        {
            DrawImplementations.BSplineImpl(controlPoints, detail);
        }
        public static void Arrowhead(Vector2 start, Vector2 end, float widthRatio = 1)
        {
            DrawImplementations.ArrowheadImpl(start, end, widthRatio);
        }
        public static void Arrowhead(float startX, float startY, float endX, float endY, float widthRatio = 1)
        {
            DrawImplementations.ArrowheadImpl(new Vector2(startX, startY), new Vector2(endX, endY), widthRatio);
        }
        public static void Arrowhead(Vector2 start, float endX, float endY, float widthRatio = 1)
        {
            DrawImplementations.ArrowheadImpl(start, new Vector2(endX, endY), widthRatio);
        }
        public static void Arrowhead(float startX, float startY, Vector2 end, float widthRatio = 1)
        {
            DrawImplementations.ArrowheadImpl(new Vector2(startX, startY), end, widthRatio);
        }
        public static void Path(Path2D path)
        {
            if (path == null)
            {
                return;
            }
            DrawImplementations.DrawPathImpl(path);
        }
        public static void PushState()
        {
            DrawImplementations.PushStateImpl();
        }
        public static void PopState()
        {
            DrawImplementations.PopStateImpl();
        }
        // public static void Ellipse2(float x, float y, float width, float? height, int segments = 36)
        // {
        //     DrawImplementations.EllipseImpl(new Vector2(x, y), width, height ?? width, segments);
        // }
    }
}
