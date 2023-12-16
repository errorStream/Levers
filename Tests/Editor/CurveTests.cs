using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CurveTests
{
    [Test]
    public void GenerateCubicBezierPointsWorks()
    {
        var points = Levers.PathComputation.GenerateCubicBezierPoints(
            new Vector2(75, 80),
            new Vector2(100, 80),
            new Vector2(110, 120),
            new Vector2(140, 125),
            0.5f);

        Debug.Log("Points: " + string.Join(", ", points.Select(x => $"({x.x}, {x.y})")));
    }

    [Test]
    public void GenerateCubicBezierPointsWorks2()
    {
        var points = Levers.PathComputation.GenerateCubicBezierPoints(
            new Vector2(10, 10),
            new Vector2(30, 60),
            new Vector2(60, 60),
            new Vector2(80, 10),
            0.5f);

        Debug.Log("Points: " + string.Join(", ", points.Select(x => $"({x.x}, {x.y})")));
    }

    [Test]
    public void GenerateCubicBezierPointsWorksPinch()
    {
        var points = Levers.PathComputation.GenerateCubicBezierPoints(
            new Vector2(10, 10),
            new Vector2(50, 80),
            new Vector2(10, 80),
            new Vector2(50, 10),
            0.5f);

        Debug.Log("Points: " + string.Join(", ", points.Select(x => $"({x.x}, {x.y})")));
    }
}
