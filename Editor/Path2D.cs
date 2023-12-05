using System;
using System.Collections.Generic;
using UnityEngine;

namespace Levers
{
    public class Path2D
    {
        private List<Vector2> _points = new List<Vector2>();

        internal Vector2[] ExportPoints()
        {
            return _points.ToArray();
        }

        internal Rect Bounds { get; private set; } = new Rect();

        private void AddPoint(Vector2 point)
        {
            _points.Add(point);

            Bounds = Rect.MinMaxRect(Mathf.Min(point.x, Bounds.xMin),
                                     Mathf.Min(point.y, Bounds.yMin),
                                     Mathf.Max(point.x, Bounds.xMax),
                                     Mathf.Max(point.y, Bounds.yMax));
        }

        private void AddPoints(IEnumerable<Vector2> points)
        {
            foreach (var point in points)
            {
                AddPoint(point);
            }
        }

        public Path2D(Vector2 start)
        {
            AddPoint(start);
        }

        public void LineTo(Vector2 position)
        {
            AddPoint(position);
        }

        public void BezierCurveTo(Vector2 controlPoint1, Vector2 controlPoint2, Vector2 position)
        {
            // TODO: Make precision adaptive
            AddPoints(PathComputation.GenerateCubicBezierPoints(_points[_points.Count - 1],
                                                                controlPoint1,
                                                                controlPoint2,
                                                                position,
                                                                10));
        }

        public void QuadraticCurveTo(Vector2 controlPoint, Vector2 position)
        {
            // TODO: Make precision adaptive
            AddPoints(PathComputation.GenerateQuadraticBezierPoints(_points[_points.Count - 1],
                                                                    controlPoint,
                                                                    position,
                                                                    10));
        }

        // NOTE: Not very confident in this code yet
        public void ArcTo(Vector2 tangent1, Vector2 tangent2, float radius)
        {
            var newPoints = PathComputation.ArcTo(_points[_points.Count - 1].x,
                                                  _points[_points.Count - 1].y,
                                                  tangent1.x,
                                                  tangent1.y,
                                                  tangent2.x,
                                                  tangent2.y,
                                                  radius);
            AddPoints(newPoints);
        }
    }
}
