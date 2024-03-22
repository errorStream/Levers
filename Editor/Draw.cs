using System.Collections.Generic;
using UnityEngine;

namespace Levers
{
    /// <summary>
    /// A collection of methods for drawing shapes and lines
    /// </summary>
    public static class Draw
    {
        /// <summary>
        /// The texture settings to use when filling shapes
        /// </summary>
        public static ITextureFill TextureFill
        {
            get => DrawImplementations.State.TextureFill;
            set => DrawImplementations.State.TextureFill = value;
        }
        /// <summary>
        /// The color to use when filling shapes
        /// </summary>
        public static Color Fill
        {
            get => DrawImplementations.State.Fill;
            set => DrawImplementations.State.Fill = value;
        }
        /// <summary>
        /// The color to use when drawing lines
        /// </summary>
        public static Color Stroke
        {
            get => DrawImplementations.State.Stroke;
            set => DrawImplementations.State.Stroke = value;
        }
        /// <summary>
        /// The width of lines
        /// </summary>
        public static float StrokeWeight
        {
            get => DrawImplementations.State.StrokeWeight;
            set => DrawImplementations.State.StrokeWeight = value;
        }
        /// <summary>
        /// The amount of anti-aliasing to use
        /// </summary>
        public static float AntiAliasing
        {
            get => DrawImplementations.State.AntiAliasing;
            set => DrawImplementations.State.AntiAliasing = value;
        }
        // CurvePrecision
        /// <summary>
        /// How close together points should be when drawing curves
        /// </summary>
        public static float CurvePrecision
        {
            get => DrawImplementations.State.CurvePrecision;
            set => DrawImplementations.State.CurvePrecision = value;
        }

        /// <summary>
        /// Draws a line from one point to another.
        /// </summary>
        /// <param name="x1">X position of the start of the line</param>
        /// <param name="y1">Y position of the start of the line</param>
        /// <param name="x2">X position of the end of the line</param>
        /// <param name="y2">Y position of the end of the line</param>
        public static void Line(float x1, float y1, float x2, float y2)
        {
            DrawImplementations.LineImpl(new Vector2(x1, y1), new Vector2(x2, y2));
        }
        /// <summary>
        /// Draws a line from one point to another.
        /// </summary>
        /// <param name="start">The start of the line</param>
        /// <param name="x2">X position of the end of the line</param>
        /// <param name="y2">Y position of the end of the line</param>
        public static void Line(Vector2 start, float x2, float y2)
        {
            DrawImplementations.LineImpl(start, new Vector2(x2, y2));
        }
        /// <summary>
        /// Draws a line from one point to another.
        /// </summary>
        /// <param name="x1">X position of the start of the line</param>
        /// <param name="y1">Y position of the start of the line</param>
        /// <param name="end">The end of the line</param>
        public static void Line(float x1, float y1, Vector2 end)
        {
            DrawImplementations.LineImpl(new Vector2(x1, y1), end);
        }
        /// <summary>
        /// Draws a line from one point to another.
        /// </summary>
        /// <param name="start">The start of the line</param>
        /// <param name="end">The end of the line</param>
        public static void Line(Vector2 start, Vector2 end)
        {
            DrawImplementations.LineImpl(start, end);
        }

        private const int DEFAULT_ELLIPSE_SEGMENTS = 36;

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="x">X position of the center of the circle</param>
        /// <param name="y">Y position of the center of the circle</param>
        /// <param name="diameter">Diameter of the circle</param>
        public static void Circle(float x, float y, float diameter)
        {
            DrawImplementations.EllipseImpl(new Vector2(x, y), diameter, diameter, DEFAULT_ELLIPSE_SEGMENTS);
        }
        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="center">Center of the circle</param>
        /// <param name="diameter">Diameter of the circle</param>
        public static void Circle(Vector2 center, float diameter)
        {
            DrawImplementations.EllipseImpl(center, diameter, diameter, DEFAULT_ELLIPSE_SEGMENTS);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="x">X position of the center of the ellipse</param>
        /// <param name="y">Y position of the center of the ellipse</param>
        /// <param name="width">Width of the ellipse</param>
        /// <param name="height">Height of the ellipse. If exluded <paramref name="width"/> is used</param>
        /// <param name="segments">Number of segments to use to draw the ellipse</param>
        public static void Ellipse(float x, float y, float width, float? height, int segments = DEFAULT_ELLIPSE_SEGMENTS)
        {
            DrawImplementations.EllipseImpl(new Vector2(x, y), width, height ?? width, segments);
        }
        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="center">Center of the ellipse</param>
        /// <param name="width">Width of the ellipse</param>
        /// <param name="height">Height of the ellipse. If exluded <paramref name="width"/> is used</param>
        /// <param name="segments">Number of segments to use to draw the ellipse</param>
        public static void Ellipse(Vector2 center, float width, float? height, int segments = DEFAULT_ELLIPSE_SEGMENTS)
        {
            DrawImplementations.EllipseImpl(center, width, height ?? width, segments);
        }
        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="center">Center of the ellipse</param>
        /// <param name="size">Width and height of the ellipse</param>
        /// <param name="segments">Number of segments to use to draw the ellipse</param>
        public static void Ellipse(Vector2 center, Vector2 size, int segments = DEFAULT_ELLIPSE_SEGMENTS)
        {
            DrawImplementations.EllipseImpl(center, size.x, size.y, segments);
        }
        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="x">X position of the center of the ellipse</param>
        /// <param name="y">Y position of the center of the ellipse</param>
        /// <param name="size">Width and height of the ellipse</param>
        /// <param name="segments">Number of segments to use to draw the ellipse</param>
        public static void Ellipse(float x, float y, Vector2 size, int segments = DEFAULT_ELLIPSE_SEGMENTS)
        {
            DrawImplementations.EllipseImpl(new Vector2(x, y), size.x, size.y, segments);
        }
        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="bounds">Bounds of the ellipse. Ellipse be as large as possible while fitting in this rect</param>
        /// <param name="segments">Number of segments to use to draw the ellipse</param>
        public static void Ellipse(Rect bounds, int segments = DEFAULT_ELLIPSE_SEGMENTS)
        {
            DrawImplementations.EllipseImpl(new Vector2(bounds.x, bounds.y), bounds.width, bounds.height, segments);
        }

        private const int DEFAULT_ARC_DETAIL = 25;
        /// <summary>
        /// Draws an arc.
        /// </summary>
        /// <param name="x">X position of the center of the arc</param>
        /// <param name="y">Y position of the center of the arc</param>
        /// <param name="w">Width of the arc</param>
        /// <param name="h">Height of the arc</param>
        /// <param name="start">Start angle of the arc in degrees</param>
        /// <param name="stop">Stop angle of the arc in degrees</param>
        /// <param name="mode">Drawing mode of the arc</param>
        /// <param name="detail">Number of segments to use to draw the arc</param>
        public static void Arc(float x, float y, float w, float h, float start, float stop, ArcDrawMode mode = default, int detail = DEFAULT_ARC_DETAIL)
        {
            DrawImplementations.ArcImpl(new Vector2(x, y), w, h, start * Mathf.Deg2Rad, stop * Mathf.Deg2Rad, mode, detail);
        }
        /// <summary>
        /// Draws an arc.
        /// </summary>
        /// <param name="center">Center of the arc</param>
        /// <param name="w">Width of the arc</param>
        /// <param name="h">Height of the arc</param>
        /// <param name="start">Start angle of the arc in degrees</param>
        /// <param name="stop">Stop angle of the arc in degrees</param>
        /// <param name="mode">Drawing mode of the arc</param>
        /// <param name="detail">Number of segments to use to draw the arc</param>
        public static void Arc(Vector2 center, float w, float h, float start, float stop, ArcDrawMode mode = default, int detail = DEFAULT_ARC_DETAIL)
        {
            DrawImplementations.ArcImpl(center, w, h, start * Mathf.Deg2Rad, stop * Mathf.Deg2Rad, mode, detail);
        }
        /// <summary>
        /// Draws an arc.
        /// </summary>
        /// <param name="center">Center of the arc</param>
        /// <param name="size">Width and height of the arc</param>
        /// <param name="start">Start angle of the arc in degrees</param>
        /// <param name="stop">Stop angle of the arc in degrees</param>
        /// <param name="mode">Drawing mode of the arc</param>
        /// <param name="detail">Number of segments to use to draw the arc</param>
        public static void Arc(Vector2 center, Vector2 size, float start, float stop, ArcDrawMode mode = default, int detail = DEFAULT_ARC_DETAIL)
        {
            DrawImplementations.ArcImpl(center, size.x, size.y, start * Mathf.Deg2Rad, stop * Mathf.Deg2Rad, mode, detail);
        }
        /// <summary>
        /// Draws an arc.
        /// </summary>
        /// <param name="x">X position of the center of the arc</param>
        /// <param name="y">Y position of the center of the arc</param>
        /// <param name="size">Width and height of the arc</param>
        /// <param name="start">Start angle of the arc in degrees</param>
        /// <param name="stop">Stop angle of the arc in degrees</param>
        /// <param name="mode">Drawing mode of the arc</param>
        /// <param name="detail">Number of segments to use to draw the arc</param>
        public static void Arc(float x, float y, Vector2 size, float start, float stop, ArcDrawMode mode = default, int detail = DEFAULT_ARC_DETAIL)
        {
            DrawImplementations.ArcImpl(new Vector2(x, y), size.x, size.y, start * Mathf.Deg2Rad, stop * Mathf.Deg2Rad, mode, detail);
        }
        /// <summary>
        /// Draws an arc.
        /// </summary>
        /// <param name="bounds">Bounds of the arc. Arc will be as large as possible while fitting in this rect</param>
        /// <param name="start">Start angle of the arc in degrees</param>
        /// <param name="stop">Stop angle of the arc in degrees</param>
        /// <param name="mode">Drawing mode of the arc</param>
        /// <param name="detail">Number of segments to use to draw the arc</param>
        public static void Arc(Rect bounds, float start, float stop, ArcDrawMode mode = default, int detail = DEFAULT_ARC_DETAIL)
        {
            DrawImplementations.ArcImpl(new Vector2(bounds.x, bounds.y), bounds.width, bounds.height, start * Mathf.Deg2Rad, stop * Mathf.Deg2Rad, mode, detail);
        }

        private const int DEFAULT_RECT_DETAIL = 8;
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="x">X position of the top-left corner of the rectangle</param>
        /// <param name="y">Y position of the top-left corner of the rectangle</param>
        /// <param name="w">Width of the rectangle</param>
        /// <param name="h">Height of the rectangle</param>
        /// <param name="tl">Radius of the top-left corner</param>
        /// <param name="tr">Radius of the top-right corner</param>
        /// <param name="br">Radius of the bottom-right corner</param>
        /// <param name="bl">Radius of the bottom-left corner</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(float x, float y, float w, float? h = null, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(x, y), w, h ?? w, tl, tr, br, bl, detail);
        }
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="position">Position of the top-left corner of the rectangle</param>
        /// <param name="w">Width of the rectangle</param>
        /// <param name="h">Height of the rectangle</param>
        /// <param name="tl">Radius of the top-left corner</param>
        /// <param name="tr">Radius of the top-right corner</param>
        /// <param name="br">Radius of the bottom-right corner</param>
        /// <param name="bl">Radius of the bottom-left corner</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(Vector2 position, float w, float? h = null, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(position, w, h ?? w, tl, tr, br, bl, detail);
        }
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="x">X position of the top-left corner of the rectangle</param>
        /// <param name="y">Y position of the top-left corner of the rectangle</param>
        /// <param name="size">Size of the rectangle</param>
        /// <param name="tl">Radius of the top-left corner</param>
        /// <param name="tr">Radius of the top-right corner</param>
        /// <param name="br">Radius of the bottom-right corner</param>
        /// <param name="bl">Radius of the bottom-left corner</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(float x, float y, Vector2 size, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(x, y), size.x, size.y, tl, tr, br, bl, detail);
        }
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="position">Position of the top-left corner of the rectangle</param>
        /// <param name="size">Size of the rectangle</param>
        /// <param name="tl">Radius of the top-left corner</param>
        /// <param name="tr">Radius of the top-right corner</param>
        /// <param name="br">Radius of the bottom-right corner</param>
        /// <param name="bl">Radius of the bottom-left corner</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(Vector2 position, Vector2 size, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(position, size.x, size.y, tl, tr, br, bl, detail);
        }
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="bounds">Bounds of the rectangle</param>
        /// <param name="tl">Radius of the top-left corner</param>
        /// <param name="tr">Radius of the top-right corner</param>
        /// <param name="br">Radius of the bottom-right corner</param>
        /// <param name="bl">Radius of the bottom-left corner</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(Rect bounds, float tl = 0, float tr = 0, float br = 0, float bl = 0, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(bounds.x, bounds.y), bounds.width, bounds.height, tl, tr, br, bl, detail);
        }

        /// <summary>
        /// Draws a rectangle with rounded corners.
        /// </summary>
        /// <param name="x">X position of the top-left corner of the rectangle</param>
        /// <param name="y">Y position of the top-left corner of the rectangle</param>
        /// <param name="w">Width of the rectangle</param>
        /// <param name="h">Height of the rectangle</param>
        /// <param name="cornerRadius">Radius of the corners</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(float x, float y, float w, float h, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(x, y), w, h, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }
        /// <summary>
        /// Draws a rectangle with rounded corners.
        /// </summary>
        /// <param name="position">Position of the top-left corner of the rectangle</param>
        /// <param name="w">Width of the rectangle</param>
        /// <param name="h">Height of the rectangle</param>
        /// <param name="cornerRadius">Radius of the corners</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(Vector2 position, float w, float h, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(position, w, h, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }
        /// <summary>
        /// Draws a rectangle with rounded corners.
        /// </summary>
        /// <param name="x">X position of the top-left corner of the rectangle</param>
        /// <param name="y">Y position of the top-left corner of the rectangle</param>
        /// <param name="size">Size of the rectangle</param>
        /// <param name="cornerRadius">Radius of the corners</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(float x, float y, Vector2 size, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(x, y), size.x, size.y, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }
        /// <summary>
        /// Draws a rectangle with rounded corners.
        /// </summary>
        /// <param name="position">Position of the top-left corner of the rectangle</param>
        /// <param name="size">Size of the rectangle</param>
        /// <param name="cornerRadius">Radius of the corners</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(Vector2 position, Vector2 size, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(position, size.x, size.y, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }
        /// <summary>
        /// Draws a rectangle with rounded corners.
        /// </summary>
        /// <param name="bounds">Bounds of the rectangle</param>
        /// <param name="cornerRadius">Radius of the corners</param>
        /// <param name="detail">Number of segments to use to draw the rectangles corners</param>
        public static void Rect(Rect bounds, float cornerRadius, int detail = DEFAULT_RECT_DETAIL)
        {
            DrawImplementations.RectImp(new Vector2(bounds.x, bounds.y), bounds.width, bounds.height, cornerRadius, cornerRadius, cornerRadius, cornerRadius, detail);
        }

        /// <summary>
        /// Draws a star.
        /// </summary>
        /// <param name="x">X position of the center of the star</param>
        /// <param name="y">Y position of the center of the star</param>
        /// <param name="innerRadius">Radius of the inner points of the star</param>
        /// <param name="outerRadius">Radius of the outer points of the star</param>
        /// <param name="points">Number of points the star has</param>
        /// <param name="rotation">Rotation of the star in degrees</param>
        public static void Star(float x, float y, float innerRadius, float outerRadius, int points, float rotation = 0f)
        {
            DrawImplementations.StarImp(x, y, innerRadius, outerRadius, points, rotation * Mathf.Deg2Rad);
        }
        /// <summary>
        /// Draws a star.
        /// </summary>
        /// <param name="center">Position of the center of the star</param>
        /// <param name="innerRadius">Radius of the inner points of the star</param>
        /// <param name="outerRadius">Radius of the outer points of the star</param>
        /// <param name="points">Number of points the star has</param>
        /// <param name="rotation">Rotation of the star in degrees</param>
        public static void Star(Vector2 center, float innerRadius, float outerRadius, int points, float rotation = 0f)
        {
            DrawImplementations.StarImp(center.x, center.y, innerRadius, outerRadius, points, rotation * Mathf.Deg2Rad);
        }
        /// <summary>
        /// Draws a star.
        /// </summary>
        /// <param name="x">X position of the center of the star</param>
        /// <param name="y">Y position of the center of the star</param>
        /// <param name="radii">List of radii for the star's points and dips</param>
        /// <param name="rotation">Rotation of the star in degrees</param>
        public static void Star(float x, float y, IReadOnlyList<float> radii, float rotation = 0f)
        {
            DrawImplementations.VariableRadiusPolygonImpl(x, y, radii, rotation * Mathf.Deg2Rad);
        }
        /// <summary>
        /// Draws a star.
        /// </summary>
        /// <param name="center">Position of the center of the star</param>
        /// <param name="radii">List of radii for the star's points and dips</param>
        /// <param name="rotation">Rotation of the star in degrees</param>
        public static void Star(Vector2 center, IReadOnlyList<float> radii, float rotation = 0f)
        {
            DrawImplementations.VariableRadiusPolygonImpl(center.x, center.y, radii, rotation * Mathf.Deg2Rad);
        }
        /// <summary> 
        /// Draws a cubic bezier curve.
        /// </summary>
        /// <param name="p1">Start point</param>
        /// <param name="c1">Start handle point</param>
        /// <param name="c2">End handle point</param>
        /// <param name="p2">End point</param>
        public static void Bezier(Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2)
        {
            DrawImplementations.CubicBezierImpl(p1, c1, c2, p2);
        }
        /// <summary>
        /// Draws a cubic bezier curve.
        /// </summary>
        /// <param name="x1">X position of the start point</param>
        /// <param name="y1">Y position of the start point</param>
        /// <param name="cx1">X position of the start handle point</param>
        /// <param name="cy1">Y position of the start handle point</param>
        /// <param name="cx2">X position of the end handle point</param>
        /// <param name="cy2">Y position of the end handle point</param>
        /// <param name="x2">X position of the end point</param>
        /// <param name="y2">Y position of the end point</param>
        public static void Bezier(float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2)
        {
            DrawImplementations.CubicBezierImpl(new Vector2(x1, y1), new Vector2(cx1, cy1), new Vector2(cx2, cy2), new Vector2(x2, y2));
        }
        /// <summary>
        /// Draws a quadratic bezier curve.
        /// </summary>
        /// <param name="p1">Start point</param>
        /// <param name="c">Handle point</param>
        /// <param name="p2">End point</param>
        public static void Bezier(Vector2 p1, Vector2 c, Vector2 p2)
        {
            DrawImplementations.QuadraticBezierImpl(p1, c, p2);
        }
        /// <summary>
        /// Draws a quadratic bezier curve.
        /// </summary>
        /// <param name="x1">X position of the start point</param>
        /// <param name="y1">Y position of the start point</param>
        /// <param name="cx">X position of the handle point</param>
        /// <param name="cy">Y position of the handle point</param>
        /// <param name="x2">X position of the end point</param>
        /// <param name="y2">Y position of the end point</param>
        public static void Bezier(float x1, float y1, float cx, float cy, float x2, float y2)
        {
            DrawImplementations.QuadraticBezierImpl(new Vector2(x1, y1), new Vector2(cx, cy), new Vector2(x2, y2));
        }
        /// <summary>
        /// Draws a B-Spline curve.
        /// </summary>
        /// <param name="controlPoints">List of control points</param>
        /// <param name="detail">Number of segments to use to draw the curve</param>
        public static void BSpline(IReadOnlyList<Vector2> controlPoints, int detail = 20)
        {
            DrawImplementations.BSplineImpl(controlPoints, detail);
        }
        /// <summary>
        /// Draws an arrow head.
        /// </summary>
        /// <param name="start">Start point of the arrow</param>
        /// <param name="end">End point of the arrow</param>
        /// <param name="widthRatio">Width of the arrow head as a ratio of the length of the arrow</param>
        public static void Arrowhead(Vector2 start, Vector2 end, float widthRatio = 1)
        {
            DrawImplementations.ArrowheadImpl(start, end, widthRatio);
        }
        /// <summary>
        /// Draws an arrow head.
        /// </summary>
        /// <param name="startX">X position of the start of the arrow</param>
        /// <param name="startY">Y position of the start of the arrow</param>
        /// <param name="endX">X position of the end of the arrow</param>
        /// <param name="endY">Y position of the end of the arrow</param>
        /// <param name="widthRatio">Width of the arrow head as a ratio of the length of the arrow</param>
        public static void Arrowhead(float startX, float startY, float endX, float endY, float widthRatio = 1)
        {
            DrawImplementations.ArrowheadImpl(new Vector2(startX, startY), new Vector2(endX, endY), widthRatio);
        }
        /// <summary>
        /// Draws an arrow head.
        /// </summary>
        /// <param name="start">Start point of the arrow</param>
        /// <param name="endX">X position of the end of the arrow</param>
        /// <param name="endY">Y position of the end of the arrow</param>
        /// <param name="widthRatio">Width of the arrow head as a ratio of the length of the arrow</param>
        public static void Arrowhead(Vector2 start, float endX, float endY, float widthRatio = 1)
        {
            DrawImplementations.ArrowheadImpl(start, new Vector2(endX, endY), widthRatio);
        }
        /// <summary>
        /// Draws an arrow head.
        /// </summary>
        /// <param name="startX">X position of the start of the arrow</param>
        /// <param name="startY">Y position of the start of the arrow</param>
        /// <param name="end">End point of the arrow</param>
        /// <param name="widthRatio">Width of the arrow head as a ratio of the length of the arrow</param>
        public static void Arrowhead(float startX, float startY, Vector2 end, float widthRatio = 1)
        {
            DrawImplementations.ArrowheadImpl(new Vector2(startX, startY), end, widthRatio);
        }
        /// <summary>
        /// Draws a path.
        /// </summary>
        /// <param name="path">Path to draw</param>
        public static void Path(Path2D path)
        {
            if (path == null)
            {
                return;
            }
            DrawImplementations.DrawPathImpl(path);
        }
        /// <summary>
        /// Push a new rendering state onto the stack, reseting the state to the default.
        /// </summary>
        public static void PushState()
        {
            DrawImplementations.PushStateImpl();
        }
        /// <summary>
        /// Pop the current rendering state from the stack, restoring the previous state.
        /// </summary>
        public static void PopState()
        {
            DrawImplementations.PopStateImpl();
        } 
    }
}
