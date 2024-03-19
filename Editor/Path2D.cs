using System;
using System.Collections.Generic;
using System.Linq;
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

        public class Partition
        {
            public float Start;
            public float End;
            public List<(Vector2, Vector2)> Edges = new List<(Vector2, Vector2)>();
            public int Depth;

            public void Clear()
            {
                Edges.Clear();
                Start = 0;
                End = 0;
            }
        }

        private List<Partition> _partitions = new List<Partition>();

        public IReadOnlyCollection<Partition> Partitions
        {
            get
            {
                if (_partitions.Count == 0)
                {
                    PerformPartition();
                }
                return _partitions;
            }
        }

        private IEnumerable<Partition> PerformPartition(Partition parent)
        {
            if (parent.Depth >= 8)
            {
                Debug.LogWarning($"Max depth reached. Path edge count: {parent.Edges.Count}");
                yield return new Partition()
                {
                    Start = parent.Start,
                    End = parent.End,
                    Edges = parent.Edges.Take(DrawImplementations.MaxPolygonVertices / 2).ToList(),
                    Depth = parent.Depth,
                };
            }
            else
            {

                var partA = new Partition()
                {
                    Start = parent.Start,
                    End = ((parent.End - parent.Start) / 2f) + parent.Start,
                    Depth = parent.Depth + 1,
                };
                var partB = new Partition()
                {
                    Start = ((parent.End - parent.Start) / 2f) + parent.Start,
                    End = parent.End,
                    Depth = parent.Depth + 1,
                };

                for (int i = 0; i < parent.Edges.Count; ++i)
                {
                    var edge = parent.Edges[i];

                    if (edge.Item1.y <= partA.End || edge.Item2.y <= partA.End)
                    {
                        partA.Edges.Add(edge);
                    }

                    if (edge.Item1.y >= partB.Start || edge.Item2.y >= partB.Start)
                    {
                        partB.Edges.Add(edge);
                    }
                }

                if (partA.Edges.Count * 2 <= DrawImplementations.MaxPolygonVertices)
                {
                    yield return partA;
                }
                else
                {
                    foreach (var partition in PerformPartition(partA))
                    {
                        yield return partition;
                    }
                }

                if (partB.Edges.Count * 2 <= DrawImplementations.MaxPolygonVertices)
                {
                    yield return partB;
                }
                else
                {
                    foreach (var partition in PerformPartition(partB))
                    {
                        yield return partition;
                    }
                }
            }
        }

        private readonly Partition _rootPartition = new Partition();

        private Vector2? _lastPoint = null;

        private void PerformPartition()
        {
            _partitions.Clear();

            if (_rootPartition.Edges.Count * 2 <= DrawImplementations.MaxPolygonVertices)
            {
                _partitions.Add(_rootPartition);
            }
            else
            {
                foreach (var partition in PerformPartition(_rootPartition))
                {
                    _partitions.Add(partition);
                }
            }
        }

        private void AddPoint(Vector2 point)
        {
            _points.Add(point);

            Bounds = Rect.MinMaxRect(Mathf.Min(point.x, Bounds.xMin),
                                     Mathf.Min(point.y, Bounds.yMin),
                                     Mathf.Max(point.x, Bounds.xMax),
                                     Mathf.Max(point.y, Bounds.yMax));

            if (_lastPoint is null)
            {
                _lastPoint = point;
            }
            else
            {
                _rootPartition.Edges.Add((_lastPoint.Value, point));
                _lastPoint = point;
            }

            _rootPartition.Start = Bounds.yMin;
            _rootPartition.End = Bounds.yMax;

            _partitions.Clear();
        }

        public void Clear()
        {
            _points.Clear();
            _partitions.Clear();
            _rootPartition.Clear();
            _lastPoint = null;
            Bounds = new Rect();
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
            PathComputation.GenerateCubicBezierPoints(p0: _points[_points.Count - 1],
                                                      p1: controlPoint1,
                                                      p2: controlPoint2,
                                                      p3: position,
                                                      threshold: DrawImplementations.State.CurvePrecision,
                                                      onPoint: AddPoint);
        }

        public void QuadraticCurveTo(Vector2 controlPoint, Vector2 position)
        {
            // TODO: Make precision adaptive
            PathComputation.GenerateQuadraticBezierPoints(_points[_points.Count - 1],
                                                          controlPoint,
                                                          position,
                                                          DrawImplementations.State.CurvePrecision,
                                                          AddPoint);
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
