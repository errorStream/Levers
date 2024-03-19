using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Levers
{
    public class Path2D
    {
        private const int _partitionPoolSize = 256;
        private static readonly Queue<Partition> _partitionPool = new Queue<Partition>(_partitionPoolSize);

        ~Path2D()
        {
            ClearPartitions();
        }

        private static Partition GetPartition()
        {
            if (_partitionPool.Count > 0)
            {
                return _partitionPool.Dequeue();
            }
            return new Partition();
        }

        private static void ReturnPartition(Partition partition)
        {
            if (_partitionPool.Count >= _partitionPoolSize)
            {
                return;
            }
            _partitionPool.Enqueue(partition);
        }

        public Path2D()
        {
            LineToAction = LineTo;
        }
        public Path2D(Vector2 start) : this()
        {
            AddPoint(start);
        }

        public void LineTo(Vector2 position)
        {
            AddPoint(position);
        }

        internal readonly Action<Vector2> LineToAction;

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

        public void Close()
        {
            if (_points.Count > 1)
            {
                AddPoint(_points[0]);
            }
            else
            {
                Debug.LogWarning("Path2D: ClosePath called on empty path");
            }
        }

        public void Clear()
        {
            _points.Clear();
            ClearPartitions();
            _rootPartition.Clear();
            _lastPoint = null;
            Bounds = null;
        }

        private void ClearPartitions()
        {
            foreach (var item in _partitions)
            {
                item.Clear();
                ReturnPartition(item);
            }
            _partitions.Clear();
        }

        private List<Vector2> _points = new List<Vector2>();

        internal Vector2[] ExportPoints()
        {
            return _points.ToArray();
        }

        internal Rect? Bounds { get; private set; } = null;

        internal class Partition
        {
            internal float Start;
            internal float End;
            internal readonly List<(Vector2, Vector2)> Edges = new List<(Vector2, Vector2)>();
            internal int Depth;

            internal void Clear()
            {
                Edges.Clear();
                Start = default;
                End = default;
                Depth = default;
            }

            public override string ToString()
            {
                return $"Partition(Start: {Start}, End: {End}, Edges: {Edges.Count}, Depth: {Depth})";
            }
        }

        private readonly List<Partition> _partitions = new List<Partition>();

        internal IReadOnlyList<Partition> Partitions
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
                var pt = GetPartition();
                pt.Start = parent.Start;
                pt.End = parent.End;
                pt.Edges.AddRange(parent.Edges.Take(DrawImplementations.MaxPolygonVertices / 2));
                pt.Depth = parent.Depth;
                yield return pt;
            }
            else
            {

                var partA = GetPartition();
                partA.Start = parent.Start;
                partA.End = ((parent.End - parent.Start) / 2f) + parent.Start;
                partA.Depth = parent.Depth + 1;
                var partB = GetPartition();
                partB.Start = ((parent.End - parent.Start) / 2f) + parent.Start;
                partB.End = parent.End;
                partB.Depth = parent.Depth + 1;

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
            ClearPartitions();

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

            Rect bounds;
            if (Bounds == null)
            {
                Bounds = bounds = new Rect(point, Vector2.zero);
            }
            else
            {
                bounds = Bounds.Value;
                Bounds = Rect.MinMaxRect(Mathf.Min(point.x, bounds.xMin),
                                         Mathf.Min(point.y, bounds.yMin),
                                         Mathf.Max(point.x, bounds.xMax),
                                         Mathf.Max(point.y, bounds.yMax));
            }

            if (_lastPoint != null) { _rootPartition.Edges.Add((_lastPoint.Value, point)); }
            _lastPoint = point;

            _rootPartition.Start = bounds.yMin;
            _rootPartition.End = bounds.yMax;

            ClearPartitions();
        }

        private void AddPoints(IEnumerable<Vector2> points)
        {
            foreach (var point in points)
            {
                AddPoint(point);
            }
        }

    }
}
