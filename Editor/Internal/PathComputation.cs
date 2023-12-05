using System.Collections.Generic;
using UnityEngine;

namespace Levers
{
    internal static class PathComputation
    {
        public static List<Vector2> ArcTo(float x0, float y0, float x1, float y1, float x2, float y2, float radius, int segments = 20)
        {
            List<Vector2> points = new List<Vector2>();

            // Current position (starting point of the arc)
            Vector2 currentPos = new Vector2(x0, y0);
            // Tangents' vectors
            Vector2 tangent1 = new Vector2(x1, y1) - currentPos;
            Vector2 tangent2 = new Vector2(x2, y2) - new Vector2(x1, y1);

            // Find the middle points between current position, tangent1 and tangent2
            Vector2 mid1 = currentPos + tangent1 / 2.0f;
            Vector2 mid2 = new Vector2(x1, y1) + tangent2 / 2.0f;

            // Find the normal vectors to the lines [current position, tangent1] and [tangent1, tangent2]
            Vector2 norm1 = new Vector2(-tangent1.y, tangent1.x).normalized;
            Vector2 norm2 = new Vector2(-tangent2.y, tangent2.x).normalized;

            // Calculate the intersection of the normals, which gives the circle center
            Vector2 center = LineIntersection(mid1, mid1 + norm1, mid2, mid2 + norm2);

            // Calculate start and end angles
            float startAngle = Mathf.Atan2(tangent1.y, tangent1.x);
            float endAngle = Mathf.Atan2(tangent2.y, tangent2.x);

            // Adjust angles for the radius sign
            if (radius < 0)
            {
                radius = -radius;
                float temp = startAngle;
                startAngle = endAngle;
                endAngle = temp;
            }

            // Normalize angles
            startAngle = NormalizeAngle(startAngle);
            endAngle = NormalizeAngle(endAngle);

            if (startAngle > endAngle)
            {
                endAngle += 2 * Mathf.PI;
            }

            // Generate the points
            float deltaAngle = (endAngle - startAngle) / segments;
            for (int i = 0; i <= segments; i++)
            {
                float angle = startAngle + i * deltaAngle;
                points.Add(new Vector2(center.x + radius * Mathf.Cos(angle), center.y + radius * Mathf.Sin(angle)));
            }

            return points;
        }

        private static Vector2 LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float denominator = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);
            if (Mathf.Abs(denominator) < 1e-6)
            {
                return Vector2.zero; // Lines are parallel or coincident
            }

            float x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / denominator;
            float y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / denominator;

            return new Vector2(x, y);
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle < 0)
            {
                angle += 2 * Mathf.PI;
            }
            return angle % (2 * Mathf.PI);
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
        internal static Vector2[] GenerateCubicBezierPoints(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int numPoints)
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
        internal static Vector2[] GenerateQuadraticBezierPoints(Vector2 p0, Vector2 p1, Vector2 p2, int numPoints)
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

        internal static List<Vector2> ApproximateCubicBSpline(IReadOnlyList<Vector2> controlPoints, int segmentsPerCurve = 10)
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
    }
}
