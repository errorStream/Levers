using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Levers.Samples
{
    /// <summary>
    /// A window to demonstrate the various drawing capabilities of Levers.
    /// </summary>
    public class DrawDemosWindow : EditorWindow
    {
        private const string AnimationsTopicName = "Animations";
        private const int WIDTH = 160;
        private const int HEIGHT = 90;
        private Matrix4x4 _originalMatrix;

        [MenuItem("Window/Levers/Samples/Draw Demos")]
        private static void ShowWindow()
        {
            var window = GetWindow<DrawDemosWindow>("Levers Draw Demos");
        }

        private float _animationTime = 0f;
        private double _lastTime;

        private LinearGradient _exampleLinearGradient;
        private RadialGradient _exampleRadialGradient;
        private Image _exampleImage;

        private void OnEnable()
        {
            _animationTime = 0f;
            _lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += UpdateAnimation;
            _exampleLinearGradient = new LinearGradient(
                new[] {
                new ColorStop(0, Color.red),
                new ColorStop(1, Color.green),
                },
                new Vector2(160, 90))
            {
                Center = new Vector2(80, 45),
                Scale = Vector2.one / 2,
                Rotation = 45,
            };
            _exampleRadialGradient = new RadialGradient(
                new[] {
                new ColorStop(0, Color.red),
                new ColorStop(1, Color.green),
                },
                new Vector2(HEIGHT, HEIGHT / 2))
            {
                Center = new Vector2(80, 45),
                Scale = Vector2.one / 2,
                Rotation = 45,
            };
            var texture = new Texture2D(2, 2);
            texture.SetPixels(new[] {
                Color.red,
                Color.green,
                Color.blue,
                Color.yellow,
            });
            texture.Apply();
            _exampleImage = new Image(texture);
        }

        private void UpdateAnimation()
        {
            if (!TopicShown(AnimationsTopicName))
            {
                return;
            }
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - _lastTime);
            _lastTime = currentTime;
            _animationTime += deltaTime;
            Repaint();
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateAnimation;
        }

        private void StartExample(string name)
        {
            if (_currentExample != null)
            {
                Debug.LogWarning("Started new example before ending previous");
            }
            // Debug.Log($"Starting example {name}");
            _currentExample = name;
            GUILayout.Label(name);
            var position = GUILayoutUtility.GetRect(WIDTH, HEIGHT);
            _originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Translate(position.min) * _originalMatrix;
            Draw.PushState();
            var val = 200f / 255f;
            Draw.Fill = new Color(val, val, val, 1f);
            Draw.Rect(0, 0, WIDTH, HEIGHT);
            Draw.PopState();
            Draw.PushState();
        }

        private object ExampleState
        {
            get
            {
                if (_currentExample != null && _exampleState.TryGetValue(_currentExample, out object value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                if (_currentExample != null)
                {
                    _exampleState[_currentExample] = value;
                }
            }
        }

        private Dictionary<string, object> _exampleState = new Dictionary<string, object>();
        private Dictionary<string, bool> _showStatus = new Dictionary<string, bool>();
        private Vector2 _scrollPos;
        private string _currentExample;

        private bool TopicShown(string name)
        {
            return _showStatus.TryGetValue(name, out bool showStatus) && showStatus;
        }

        private bool Topic(string name)
        {
            return _showStatus[name] = EditorGUILayout.Foldout(_showStatus.TryGetValue(name, out bool showStatus) && showStatus, name);
        }

        private void EndExample()
        {
            _currentExample = null;
            GUI.matrix = _originalMatrix;
            Draw.PopState();
        }

        private void OnGUI()
        {
            const float HALF_PI = Mathf.PI / 2;
            const float QUARTER_PI = Mathf.PI / 4;
            const float PI = Mathf.PI;
            const float TWO_PI = Mathf.PI * 2;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (Topic("Ellipse"))
            {
                {
                    StartExample("White ellipse with black outline in middle of a gray canvas");
                    Draw.Stroke = Color.black;
                    Draw.Ellipse(56, 46, 55, 55);
                    EndExample();
                }
                {
                    StartExample("Other ellipses");
                    Draw.Stroke = Color.black;
                    Draw.Fill = Color.yellow;
                    Draw.Ellipse(40, 40, 30, 10);
                    Draw.Fill = Color.green;
                    Draw.Ellipse(150, 45, 20, HEIGHT);
                    EndExample();
                }
            }
            if (Topic("Arc"))
            {
                {
                    StartExample("Shattered outline of ellipse with a quarter of a white circle bottom-right");
                    Draw.Stroke = Color.black;
                    Draw.Arc(50, 55, 50, 50, 0, 90);
                    Draw.Fill = Color.clear;
                    Draw.Arc(50, 55, 60, 60, 90, 180);
                    Draw.Arc(50, 55, 70, 70, 180, 225);
                    Draw.Arc(50, 55, 80, 80, 225, 360);
                    EndExample();
                }
                {
                    StartExample("White ellipse with top right quarter missing");
                    Draw.Arc(50, 50, 80, 80, 0, 225, ArcDrawMode.Center);
                    EndExample();
                }
                {
                    StartExample("White ellipse with black outline with top right missing");
                    Draw.Stroke = Color.black;
                    Draw.Arc(50, 50, 80, 80, 0, 225, ArcDrawMode.Open);
                    EndExample();
                }
                {
                    StartExample("White open arc with black outline with top right missing");
                    Draw.Stroke = Color.black;
                    Draw.Arc(50, 50, 80, 80, 0, 225, ArcDrawMode.Chord);
                    EndExample();
                }
                {
                    StartExample("White ellipse with top right quarter missing with black outline around the shape");
                    Draw.Stroke = Color.black;
                    Draw.Arc(50, 50, 80, 80, 0, 225, ArcDrawMode.Pie);
                    EndExample();
                }
            }
            if (Topic("Rect"))
            {
                {
                    // Draw a rectangle at location (30, 20) with a width and height of 55.
                    StartExample("White rect with black outline in mid-right of canvas");
                    Draw.Stroke = Color.black;
                    Draw.Rect(30, 20, 55, 55);
                    EndExample();
                }
                {
                    // Draw a rectangle with rounded corners, each having a radius of 20.
                    StartExample("White rect with black outline and round edges in mid-right of canvas");
                    Draw.Stroke = Color.black;
                    Draw.Rect(30, 20, 55, 55, 20);
                    EndExample();
                }
                {
                    // Draw a rectangle with rounded corners having the following radii:
                    StartExample("White rect with black outline and round edges of different radii");
                    Draw.Stroke = Color.black;
                    // top-left = 20, top-right = 15, bottom-right = 10, bottom-left = 5.
                    Draw.Rect(30, 20, 55, 55, 20, 15, 10, 5);
                    EndExample();
                }
            }
            if (Topic("Circle"))
            {
                {
                    StartExample("White circle with black outline in mid of gray canvas");
                    Draw.Stroke = Color.black;
                    Draw.Circle(30, 30, 20);
                    EndExample();
                }
            }
            if (Topic("Line"))
            {
                {
                    StartExample("A 78 pixels long line running from mid-top to bottom-right of canvas");
                    Draw.Stroke = Color.black;
                    Draw.Line(30, 20, 85, 75);
                    EndExample();
                }
                {
                    StartExample("A thick 78 pixels long line running from mid-top to bottom-right of canvas");
                    Draw.Stroke = Color.black;
                    Draw.StrokeWeight = 10;
                    Draw.Line(30, 30, 75, 50);
                    EndExample();
                }
                {
                    StartExample("3 lines of various stroke colors. Form top, bottom and right sides of a square");
                    Draw.Stroke = Color.black;
                    Draw.Line(30, 20, 85, 20);
                    var val = 126f / 255f;
                    Draw.Stroke = new Color(val, val, val, 1f);
                    Draw.Line(85, 20, 85, 75);
                    Draw.Stroke = Color.white;
                    Draw.Line(85, 75, 30, 75);
                    EndExample();
                }
                {
                    StartExample("White rect at center with dark charcoal grey outline.");
                    Draw.Stroke = new Color(51f / 255f, 51f / 255f, 51f / 255f, 1f);
                    Draw.StrokeWeight = 4;
                    Draw.Rect(20, 20, 60, 60);
                    EndExample();
                }
                void BSplineCurveDemo(string name, IReadOnlyList<Vector2> points)
                {
                    StartExample(name);
                    Draw.Fill = Color.clear;
                    Draw.Stroke = Color.gray;
                    for (int i = 0; i < points.Count - 1; ++i)
                    {
                        Draw.Line(points[i], points[i + 1]);
                    }
                    Draw.Fill = Color.green;
                    Draw.Stroke = Color.clear;
                    foreach (var item in points)
                    {
                        Draw.Circle(item, 5);
                    }
                    Draw.Fill = Color.clear;
                    Draw.Stroke = Color.red;
                    Draw.BSpline(points);
                    EndExample();
                }
                BSplineCurveDemo("BSpline curve",
                                 new[] {
                                     new Vector2(9.5f, 9.5f),
                                     new Vector2(9.5f, 9.5f),
                                     new Vector2(59, 309.5f),
                                     new Vector2(309.5f, 259.5f),
                                     new Vector2(359.5f, 409.5f),
                                     new Vector2(509.5f, 29.5f),
                                     new Vector2(309.5f, 109.5f),
                                     new Vector2(309.5f, 109.5f)
                                 }.Select(p => p / 5f).ToArray());
                BSplineCurveDemo("BSpline curve line",
                                 new[] {
                                     new Vector2(265.0f, 140.5f),
                                     new Vector2(265.0f, 140.5f),
                                     new Vector2(265.0f, 132.4f),
                                     new Vector2(265.0f, 124.4f),
                                     new Vector2(265.0f, 116.7f),
                                     new Vector2(265.0f, 116.7f),
                                 }.Select(p => p / 2f).ToArray());
            }
            if (Topic("Other Examples"))
            {
                {
                    StartExample("Interlaced rectangles");
                    Draw.Stroke = Color.clear;
                    Draw.AntiAliasing = 0;
                    var thickness = 10;
                    for (var i = 0; i < HEIGHT; i += thickness)
                    {
                        Draw.Fill = new Color(129f / 255f, 206f / 255f, 15f / 255f);
                        Draw.Rect(0, i, WIDTH, thickness / 2f);
                        Draw.Fill = Color.white;
                        Draw.Rect(i, 0, thickness / 2f, HEIGHT);
                    }
                    EndExample();
                }
                {
                    StartExample("Grayscale targets");
                    void DrawTarget(float xloc, float yloc, float size, int num)
                    {
                        var grayValues = 255f / num / 255f;
                        var steps = size / num;
                        for (var i = 0; i < num; i++)
                        {
                            var val = i * grayValues;
                            Draw.Stroke = Color.clear;
                            Draw.Fill = new Color(val, val, val, 1f);
                            Draw.Ellipse(xloc, yloc, size - i * steps, size - i * steps);
                        }
                    }
                    DrawTarget(WIDTH * 0.25f, HEIGHT * 0.4f, 50, 4);
                    DrawTarget(WIDTH * 0.5f, HEIGHT * 0.5f, 75, 10);
                    DrawTarget(WIDTH * 0.75f, HEIGHT * 0.3f, 30, 6);
                    EndExample();
                }
                {
                    StartExample("Recursive circle line");
                    void DrawCircle(float x, float radius, int level)
                    {
                        var tt = ((126 * level) / 4.0f) / 255f;
                        Draw.Fill = new Color(tt, tt, tt, 1);
                        Draw.Ellipse(x, HEIGHT / 2, radius * 2, radius * 2);
                        if (level > 1)
                        {
                            level = level - 1;
                            DrawCircle(x - radius / 2, radius / 2, level);
                            DrawCircle(x + radius / 2, radius / 2, level);
                        }
                    }
                    Draw.Stroke = Color.clear;
                    DrawCircle(WIDTH / 2, 45, 6);
                    EndExample();
                }

                {
                    StartExample("Arrow head");
                    Draw.Stroke = Color.clear;
                    Draw.Fill = Color.black;
                    Draw.Arrowhead(20, 20, 100, 70, 0.4f);
                    Draw.Fill = Color.red;
                    Draw.Circle(20, 20, 7);
                    Draw.Fill = Color.blue;
                    Draw.Circle(100, 70, 7);
                    EndExample();
                }
            }
            if (Topic(AnimationsTopicName))
            {
                {
                    StartExample("Scan line");
                    Draw.Fill = Color.black;
                    Draw.Stroke = Color.clear;
                    Draw.Rect(0, 0, WIDTH, HEIGHT);
                    Draw.Stroke = Color.white;
                    var pos = (_animationTime * HEIGHT * 0.25f) % HEIGHT;
                    Draw.Line(0, pos, WIDTH, pos);
                    EndExample();
                }
                {
                    StartExample("Function Plot");

                    var position = new Rect(0, 0, WIDTH, HEIGHT);
                    Func<float, float> f = x => Mathf.Sin(x / 10 * Mathf.PI) * 10;
                    var settings = new Components.FunctionPlotSettings
                    {
                        MinX = -10,
                        MaxX = 10,
                        MinY = -10,
                        MaxY = 10,
                        Snap = 1.1f,
                        LabelBackgroundColor = new Color(0.1f, 0.15f, 0.1f, 0.7f),
                        Step = 5,
                        BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 1),
                        LineThickness = 1.5f,
                        LineColor = Color.yellow,
                        SnapToEnds = true,
                        XFormat = "X {0:0.00}",
                        YFormat = "Y {0:0.00}",
                        LabelStyle = GUI.skin.label,
                        GridSpacing = Vector2.one * 2,
                        GridColor = new Color(0.2f, 0.2f, 0.3f, 1f),
                    };
                    Components.FunctionPlot(position, f, settings);

                    EndExample();
                }
                {
                    StartExample("Function Plot with Default Settings");

                    var position = new Rect(0, 0, WIDTH, HEIGHT);
                    Func<float, float> f = x => x * x;
                    Components.FunctionPlot(position, f);

                    EndExample();
                }

                var normalizedSine = (Mathf.Sin(_animationTime) + 1f) * 0.5f;
                void GradientExample(string name, ITextureFill gradient, Vector2? center = null, Vector2? scale = null, float? rotation = null)
                {
                    StartExample(name);
                    Draw.Fill = Color.white;
                    Draw.Stroke = Color.clear;
                    Draw.TextureFill = gradient;
                    var size = HEIGHT;
                    gradient.Center = center ?? new Vector2(WIDTH / 2, HEIGHT / 2);
                    gradient.Scale = scale ?? Vector2.one;
                    gradient.Rotation = rotation ?? 0;
                    Draw.Rect((WIDTH / 2) - (size / 2), (HEIGHT / 2) - (size / 2), size);
                    Draw.TextureFill = null;
                    Draw.Circle(WIDTH / 2, HEIGHT / 2, 10);
                    EndExample();
                }

                GradientExample("Default linear gradient", _exampleLinearGradient);
                GradientExample("Scaling linear gradient", _exampleLinearGradient, scale: Vector2.one * ((normalizedSine * 0.8f) + 0.2f));
                GradientExample("Moving linear gradient", _exampleLinearGradient, center: new Vector2((WIDTH / 2) + ((normalizedSine - 0.5f) * 200), HEIGHT / 2));
                GradientExample("Rotating linear gradient", _exampleLinearGradient, rotation: _animationTime * 30);
                GradientExample("All transforms linear gradient", _exampleLinearGradient,
                                center: new Vector2((WIDTH / 2) + ((normalizedSine - 0.5f) * 200), HEIGHT / 2),
                                scale: Vector2.one * ((normalizedSine * 0.8f) + 0.2f),
                                rotation: _animationTime * 30);
                GradientExample("Default radial gradient", _exampleRadialGradient);
                GradientExample("Scaling radial gradient", _exampleRadialGradient, scale: Vector2.one * ((normalizedSine * 0.8f) + 0.2f));
                GradientExample("Moving radial gradient", _exampleRadialGradient, center: new Vector2((WIDTH / 2) + ((normalizedSine - 0.5f) * 200), HEIGHT / 2));
                GradientExample("Rotating radial gradient", _exampleRadialGradient, rotation: _animationTime * 30);
                GradientExample("All transforms radial gradient", _exampleRadialGradient,
                                center: new Vector2((WIDTH / 2) + ((normalizedSine - 0.5f) * 200), HEIGHT / 2),
                                scale: Vector2.one * ((normalizedSine * 0.8f) + 0.2f),
                                rotation: _animationTime * 30);

                {
                    StartExample("Rotating stars");
                    Draw.Star(WIDTH * 0.2f, HEIGHT * 0.5f, 5, 20, 3, _animationTime * 2 * Mathf.Rad2Deg);
                    Draw.Star(WIDTH * 0.5f, HEIGHT * 0.5f, 20, 30, 40, _animationTime * Mathf.Rad2Deg);
                    Draw.Star(WIDTH * 0.8f, HEIGHT * 0.5f, 10, 20, 5, _animationTime * 3 * Mathf.Rad2Deg);
                    EndExample();
                }

                // {
                //     StartExample("Rotating variable radius polygon");
                //     const int steps = 64;
                //     var radii = new float[steps];
                //     for (int i = 0; i < steps; ++i)
                //     {
                //         var radius = Mathf.Sqrt(i / ((float)(steps - 1)));
                //         radii[i] = radius;
                //     }
                //     ExtraGUI.VariableRadiusPolygon(WIDTH * 0.5f, HEIGHT * 0.5f, radii, _animationTime);
                //     EndExample();
                // }

                {
                    StartExample("Cubic bezier fan following cursor");
                    Draw.Fill = Color.black;
                    Draw.Stroke = Color.clear;
                    Draw.Rect(0, 0, WIDTH, HEIGHT);
                    Draw.Stroke = Color.white;
                    Draw.StrokeWeight = 1.5f;
                    var mouseX = Mathf.Clamp(Event.current.mousePosition.x, 0, WIDTH);
                    for (var i = 0; i < 40; i += 5)
                    {
                        Draw.Bezier(
                          mouseX - (i / 2.0f), 8 + i,
                          82, 4,
                          85, 60,
                          48 - (i / 16.0f), 60 + (i / 8.0f)
                        );
                    }
                    EndExample();
                }
                {
                    StartExample("Quadratic bezier fan following cursor");
                    Draw.Fill = Color.black;
                    Draw.Stroke = Color.clear;
                    Draw.Rect(0, 0, WIDTH, HEIGHT);
                    Draw.Stroke = Color.white;
                    Draw.StrokeWeight = 1.5f;
                    var mouseX = Mathf.Clamp(Event.current.mousePosition.x, 0, WIDTH);
                    for (var i = 0; i < 40; i += 5)
                    {
                        Draw.Bezier(
                          mouseX - (i / 2.0f), 8 + i,
                          84, 20,
                          48 - (i / 16.0f), 60 + (i / 8.0f)
                        );
                    }
                    EndExample();
                }
                {
                    StartExample("Gradient Star");
                    Draw.Fill = Color.white;
                    Draw.Stroke = Color.clear;
                    Draw.TextureFill = new LinearGradient(
                        new[] {
                            new ColorStop(0, Color.yellow),
                            new ColorStop(1, new Color(1f, 0.4f, 0.8f)),
                            new ColorStop(1, new Color(0.6f, 0.1f, 1f)),
                              },
                        new Vector2(160, 90))
                    {
                        Center = new Vector2(WIDTH / 2, HEIGHT / 2),
                        Scale = Vector2.one,
                        Rotation = 0,
                    };
                    Draw.Star(x: ((Mathf.Sin(_animationTime * 2) * 0.25f) + 0.5f) * WIDTH,
                              y: ((Mathf.Cos(_animationTime * 2) * 0.25f) + 0.5f) * HEIGHT,
                              innerRadius: 10,
                              outerRadius: 20,
                              points: 5);
                    EndExample();
                }
                {
                    StartExample("Gradient Heart");
                    Draw.Fill = Color.white;
                    Draw.Stroke = Color.clear;
                    Draw.TextureFill = new LinearGradient(
                        new[] {
                            new ColorStop(0, Color.yellow),
                            new ColorStop(1, new Color(1f, 0.4f, 0.8f)),
                            new ColorStop(1, new Color(0.6f, 0.1f, 1f)),
                              },
                        new Vector2(160, 90))
                    {
                        Center = new Vector2(WIDTH / 2, HEIGHT / 2),
                        Scale = Vector2.one,
                        Rotation = 0,
                    };
                    var x = ((Mathf.Sin(_animationTime * 2) * 0.25f) + 0.5f) * WIDTH;
                    var y = ((Mathf.Cos(_animationTime * 2) * 0.2f) + 0.5f) * HEIGHT - 8;
                    var path = new Path2D(new Vector2(4 + x, -9 + y));
                    path.BezierCurveTo(new Vector2(4 + x, -11 + y), new Vector2(2 + x, -17 + y), new Vector2(-7 + x, -17 + y));
                    path.BezierCurveTo(new Vector2(-22 + x, -17 + y), new Vector2(-22 + x, 2.5f + y), new Vector2(-22 + x, 2.5f + y));
                    path.BezierCurveTo(new Vector2(-22 + x, 11 + y), new Vector2(-12 + x, 22 + y), new Vector2(4 + x, 31 + y));
                    path.BezierCurveTo(new Vector2(22 + x, 22 + y), new Vector2(32 + x, 11 + y), new Vector2(32 + x, 2.5f + y));
                    path.BezierCurveTo(new Vector2(32 + x, 2.5f + y), new Vector2(32 + x, -17 + y), new Vector2(17 + x, -17 + y));
                    path.BezierCurveTo(new Vector2(9 + x, -17 + y), new Vector2(4 + x, -11 + y), new Vector2(4 + x, -9 + y));
                    Draw.Path(path);
                    EndExample();
                }
                {
                    StartExample("The Glob");
                    Draw.Fill = Color.white;
                    Draw.Stroke = Color.clear;
                    Draw.Fill = Color.white;
                    Draw.Stroke = Color.clear;
                    Draw.TextureFill = new RadialGradient(
                        new[] {
                            new ColorStop(0, new Color32(201, 230, 85, 255)),
                            new ColorStop(0.15f, new Color32(201, 230, 85, 255)),
                            new ColorStop(1, new Color32(10, 209, 0, 255)),
                              },
                        new Vector2(90, 90))
                    {
                        Center = new Vector2(WIDTH / 2, HEIGHT / 2),
                    };

                    var radii = new float[128];
                    for (var i = 0; i < radii.Length; i++)
                    {
                        var norm = i / (float)radii.Length;
                        var size = (Mathf.Sin((norm + (_animationTime / 7)) * Mathf.PI * 2 * 2) / 2f * 0.5f) + 1;
                        var hfValNorm = (Mathf.Sin((norm + (_animationTime / 4)) % 1f * Mathf.PI * 2 * 4 * size) * 0.5f) + 0.5f;
                        radii[i] = (hfValNorm * 20) + 20;
                    }
                    Draw.Star(x: WIDTH / 2,
                              y: HEIGHT / 2,
                              radii: radii);
                    EndExample();
                }
            }
            if (Topic("Path"))
            {
                {
                    StartExample("Polygon");
                    Draw.Fill = Color.green;
                    Draw.Stroke = Color.clear;
                    var path = new Path2D(new Vector2(30, 20));
                    path.LineTo(new Vector2(75, 20));
                    path.LineTo(new Vector2(75, 5));
                    path.LineTo(new Vector2(85, 5));
                    path.LineTo(new Vector2(85, 75));
                    path.LineTo(new Vector2(30, 75));
                    path.Close();
                    Draw.Path(path);
                    EndExample();
                }

                {
                    StartExample("Polygon Demo without Anti-Aliasing");
                    Draw.Fill = Color.green;
                    Draw.Stroke = Color.clear;
                    Draw.AntiAliasing = 0;
                    var path = new Path2D(new Vector2(30, 20));
                    path.LineTo(new Vector2(75, 20));
                    path.LineTo(new Vector2(75, 5));
                    path.LineTo(new Vector2(85, 5));
                    path.LineTo(new Vector2(85, 75));
                    path.LineTo(new Vector2(30, 75));
                    path.Close();
                    Draw.Path(path);
                    EndExample();
                }

                {
                    StartExample("Heart");
                    Draw.Fill = Color.red;
                    Draw.Stroke = Color.clear;
                    var path = new Path2D(new Vector2(32, 15));
                    path.BezierCurveTo(new Vector2(32, 13), new Vector2(30, 7), new Vector2(20, 7));
                    path.BezierCurveTo(new Vector2(5, 7), new Vector2(5, 26.5f), new Vector2(5, 26.5f));
                    path.BezierCurveTo(new Vector2(5, 35), new Vector2(15, 46), new Vector2(32, 55));
                    path.BezierCurveTo(new Vector2(50, 46), new Vector2(60, 35), new Vector2(60, 26.5f));
                    path.BezierCurveTo(new Vector2(60, 26.5f), new Vector2(60, 7), new Vector2(45, 7));
                    path.BezierCurveTo(new Vector2(37, 7), new Vector2(32, 13), new Vector2(32, 15));
                    Draw.Path(path);
                    EndExample();
                }

                {
                    StartExample("Heart with Outline");
                    Draw.Fill = Color.red;
                    Draw.Stroke = new Color(0.6f, 0.1f, 1f);
                    Draw.StrokeWeight = 4;
                    var path = new Path2D(new Vector2(32, 15));
                    path.BezierCurveTo(new Vector2(32, 13), new Vector2(30, 7), new Vector2(20, 7));
                    path.BezierCurveTo(new Vector2(5, 7), new Vector2(5, 26.5f), new Vector2(5, 26.5f));
                    path.BezierCurveTo(new Vector2(5, 35), new Vector2(15, 46), new Vector2(32, 55));
                    path.BezierCurveTo(new Vector2(50, 46), new Vector2(60, 35), new Vector2(60, 26.5f));
                    path.BezierCurveTo(new Vector2(60, 26.5f), new Vector2(60, 7), new Vector2(45, 7));
                    path.BezierCurveTo(new Vector2(37, 7), new Vector2(32, 13), new Vector2(32, 15));
                    Draw.Path(path);
                    EndExample();
                }

                {
                    StartExample("Arc with Arm");
                    Draw.Fill = Color.clear;
                    Draw.Stroke = Color.black;
                    Draw.StrokeWeight = 2;
                    var path = new Path2D(new Vector2(65, 10));
                    path.ArcTo(new Vector2(65, 65), new Vector2(-10, 10), 20);
                    Draw.Path(path);
                    EndExample();
                }

                {
                    StartExample("Triangular Triad Hole");
                    var path = new Path2D(new Vector2(7, 37));
                    path.LineTo(new Vector2(47, 2));
                    path.LineTo(new Vector2(112, 57));
                    path.LineTo(new Vector2(22, 57));
                    path.LineTo(new Vector2(87, 2));
                    path.LineTo(new Vector2(127, 37));
                    Draw.Fill = new Color(0, 0.8f, 0.1f);
                    Draw.Stroke = Color.clear;
                    Draw.Path(path);
                    EndExample();
                }

                {
                    StartExample("Complex Flower");

                    Draw.Fill = Color.blue;
                    Draw.Stroke = Color.green;
                    Draw.StrokeWeight = 1.5f;

                    var center = new Vector2(50, 50);
                    var petalCount = 16;
                    var petalLength = 39;
                    var petalWidth = 30;

                    for (int i = 0; i < petalCount; i++)
                    {
                        var angle = i * 2 * Mathf.PI / petalCount;
                        var petalStart = new Vector2(
                            center.x + petalLength * Mathf.Cos(angle),
                            center.y + petalLength * Mathf.Sin(angle)
                            );

                        var path = new Path2D(center);
                        path.BezierCurveTo(
                        new Vector2(center.x + petalWidth * Mathf.Cos(angle - Mathf.PI / 8), center.y + petalWidth * Mathf.Sin(angle - Mathf.PI / 8)),
                        new Vector2(petalStart.x, petalStart.y),
                        petalStart
                        );

                        path.BezierCurveTo(
                            new Vector2(petalStart.x, petalStart.y),
                            new Vector2(center.x + petalWidth * Mathf.Cos(angle + Mathf.PI / 8), center.y + petalWidth * Mathf.Sin(angle + Mathf.PI / 8)),
                            center
                            );

                        Draw.Path(path);
                    }

                    EndExample();
                }

                {
                    StartExample("Bars");

                    Draw.Fill = new Color(0.5f, 0.1f, 0.6f);
                    Draw.Stroke = Color.clear;

                    var barCount = 60;
                    var barWidth = 2;
                    var barHeight = 50;

                    var start = new Vector2(10, 10);

                    var path = new Path2D(start);

                    for (int i = 0; i < barCount; i += 2)
                    {
                        path.LineTo(new Vector2(start.x + (i * barWidth),
                                                start.y + barHeight));
                        path.LineTo(new Vector2(start.x + ((i + 1) * barWidth),
                                                start.y + barHeight));
                        path.LineTo(new Vector2(start.x + ((i + 1) * barWidth),
                                                start.y));
                        path.LineTo(new Vector2(start.x + ((i + 2) * barWidth),
                                                start.y));
                    }
                    path.LineTo(new Vector2(start.x + (barCount * barWidth),
                                            start.y + barHeight + 5));
                    path.LineTo(new Vector2(start.x - 5, start.y + barHeight + 5));
                    path.LineTo(start);

                    Draw.Path(path);

                    EndExample();
                }
            }

            if (Topic("Texture Fill"))
            {
                {
                    StartExample("Image Fill");
                    Draw.Fill = Color.white;
                    Draw.Stroke = Color.clear;
                    Draw.TextureFill = _exampleImage;
                    _exampleImage.Scale = Vector2.one;
                    Draw.Rect(0, 0, 64, 64);
                    EndExample();
                }

                {
                    StartExample("Image Fill Scaled");
                    Draw.Fill = Color.white;
                    Draw.Stroke = Color.clear;
                    Draw.TextureFill = _exampleImage;
                    _exampleImage.Scale = new Vector2(8f, 8f);
                    _exampleImage.Texture.filterMode = FilterMode.Point;
                    Draw.Rect(4, 4, 64, 64);
                    EndExample();
                }
            }

            if (Topic("Value Input"))
            {
                {
                    StartExample("Point in Unit Circle Input");

                    var position = new Rect(0, 0, WIDTH, HEIGHT);
                    ExampleState = Components.PointInUnitCircle(position, (ExampleState as Vector2?) ?? Vector2.zero);

                    EndExample();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
