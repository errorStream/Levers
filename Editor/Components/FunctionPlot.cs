using System;
using UnityEngine;

namespace Levers
{
    public static partial class Components
    {
        public class FunctionPlotSettings
        {
            /// <summary>
            /// The minimum x value to plot
            /// </summary>
            public float MinX = 0;
            /// <summary>
            /// The maximum x value to plot
            /// </summary>
            public float MaxX = 1;
            /// <summary>
            /// The smallest computed y value the graph can contain
            /// </summary>
            public float MinY = 0;
            /// <summary>
            /// The largest computed y value the graph can contain
            /// </summary>
            public float MaxY = 1;

            private float _snap = 0f;
            /// <summary>
            /// The size of the increment to snap the indicator to
            /// </summary>
            public float Snap
            {
                get => _snap < 0 ? 0 : _snap;
                set => _snap = (value < 0) ? 0 : value;
            }
            /// <summary>
            /// The color of the label box background
            /// </summary>
            public Color LabelBackgroundColor = new Color(0.1f, 0.15f, 0.1f, 0.7f);

            /// <summary>
            /// The number of pixels to step when drawing the graph. A smaller value will result in a smoother graph but will be slower to draw.
            /// </summary>
            public int Step = 5;
            /// <summary>
            /// The background color of the graph
            /// </summary>
            public Color BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 1);
            /// <summary>
            /// The thickness of the value line
            /// </summary>
            public float LineThickness = 1.5f;
            /// <summary>
            /// The color of the value line
            /// </summary>
            public Color LineColor = Color.yellow;
            /// <summary>
            /// Should the indicator snap to the ends of the graph if it gets close enough?
            /// </summary>
            public bool SnapToEnds = true;

            /// <summary>
            /// The format string for the x value label
            /// </summary>
            public string XFormat = "X {0:0.00}";

            /// <summary>
            /// The format string for the y value label
            /// </summary>
            public string YFormat = "Y {0:0.00}";

            /// <summary>
            /// The style of the x and y value labels
            /// </summary>
            public GUIStyle LabelStyle = GUI.skin.label;

            private Vector2 _gridSpacing = new Vector2(0, 0);

            /// <summary>
            /// The color of the grid lines
            /// </summary>
            public Color GridColor = new Color(1f, 1f, 1f, 0.6f);

            /// <summary>
            /// At what steps are the grid lines drawn?
            ///
            /// If the x value is 0, the grid lines will not be drawn.
            /// </summary>
            public Vector2 GridSpacing
            {
                get => Vector2.Max(Vector2.zero, _gridSpacing);
                set => _gridSpacing = Vector2.Max(Vector2.zero, value);
            }

            internal static readonly FunctionPlotSettings Default = new FunctionPlotSettings();
        }

        /// <summary>
        /// Draws a graph of a function in the given rect
        /// </summary>
        /// <param name="position">The rect to draw the graph in</param>
        /// <param name="function">The function to plot</param>
        /// <param name="settings">The settings for the graph</param>
        public static void FunctionPlot(Rect position, Func<float, float> function, FunctionPlotSettings settings = null)
        {
            settings ??= FunctionPlotSettings.Default;

            Vector2 LocalToGlobal(Vector2 local)
            {
                var x = (Mathf.InverseLerp(settings.MinX, settings.MaxX, local.x) * position.width) + position.x;
                var y = position.y + position.height - (Mathf.InverseLerp(settings.MinY, settings.MaxY, local.y) * position.height);
                return new Vector2(x, y);
            }

            Vector2 GlobalToLocal(Vector2 global)
            {
                var x = Mathf.Lerp(settings.MinX, settings.MaxX, (global.x - position.x) / position.width);
                var y = Mathf.Lerp(settings.MinY, settings.MaxY, 1 - ((global.y - position.y) / position.height));
                return new Vector2(x, y);
            }

            float Snap(float x)
            {
                if (settings.Snap == 0)
                {
                    return x;
                }
                var snappedX = Mathf.Round(x / settings.Snap) * settings.Snap;
                if (settings.SnapToEnds)
                {
                    if (Mathf.Abs(snappedX - settings.MinX) < (settings.Snap / 2f))
                    {
                        snappedX = settings.MinX;
                    }
                    if (Mathf.Abs(snappedX - settings.MaxX) < (settings.Snap / 2f))
                    {
                        snappedX = settings.MaxX;
                    }
                }
                return snappedX;
            }

            /* Background */
            {
                Draw.PushState();
                Draw.Fill = settings.BackgroundColor;
                Draw.Stroke = Color.clear;
                Draw.AntiAliasing = 0;
                Draw.Rect(position);
                Draw.PopState();
            }

            if (settings.MaxX <= settings.MinX || settings.MaxY <= settings.MinY)
            {
                GUI.Label(position, "Invalid range. MaxX and MaxY must be greater than MinX and MinY respectively.");
                return;
            }

            /* Grid lines */
            {
                Draw.PushState();
                Draw.Fill = Color.clear;
                Draw.Stroke = settings.GridColor;
                Draw.StrokeWeight = 1f;
                Draw.AntiAliasing = 0;
                var xGridTooTight = (settings.GridSpacing.x < 0.0001f)
                    || (((settings.MaxX - settings.MinX) / settings.GridSpacing.x) > (position.width / 2f))
                    ;
                if (!xGridTooTight)
                {
                    void DrawXLine(float i)
                    {
                        var xPos = LocalToGlobal(new Vector2(i, settings.MinY)).x;
                        Draw.Line(Mathf.Floor(xPos) + 0.5f,
                                  Mathf.Floor(position.yMin) + 0.5f,
                                  Mathf.Floor(xPos) + 0.5f,
                                  Mathf.Floor(position.yMax) + 0.5f);
                    }
                    for (float i = 0; i <= settings.MaxX; i += settings.GridSpacing.x)
                    {
                        DrawXLine(i);
                    }
                    for (float i = -settings.GridSpacing.x; i >= settings.MinX; i -= settings.GridSpacing.x)
                    {
                        DrawXLine(i);
                    }
                }
                var yGridTooTight = (settings.GridSpacing.y < 0.0001f)
                    || (((settings.MaxY - settings.MinY) / settings.GridSpacing.y) > (position.height / 2f))
                    ;
                if (!yGridTooTight)
                {
                    void DrawYLine(float i)
                    {
                        var yPos = LocalToGlobal(new Vector2(settings.MinX, i)).y;
                        Draw.Line(
                            Mathf.Floor(position.xMin) + 0.5f,
                            Mathf.Floor(yPos) + 0.5f,
                            Mathf.Floor(position.xMax) + 0.5f,
                            Mathf.Floor(yPos) + 0.5f);
                    }
                    for (float i = 0; i <= settings.MaxY; i += settings.GridSpacing.y)
                    {
                        DrawYLine(i);
                    }
                    for (float i = -settings.GridSpacing.y; i >= settings.MinY; i -= settings.GridSpacing.y)
                    {
                        DrawYLine(i);
                    }
                }
                Draw.PopState();
            }

            /* Value line */
            {
                Draw.PushState();
                Draw.Stroke = settings.LineColor;
                Draw.Fill = Color.clear;
                Draw.StrokeWeight = settings.LineThickness;
                var path = new Path2D();
                void AddPoint(int i)
                {
                    var localX = Mathf.Lerp(settings.MinX, settings.MaxX, i / (float)position.width);
                    var localY = function(localX);
                    path.LineTo(LocalToGlobal(new Vector2(localX, localY)));
                }
                AddPoint(0);
                var widthI = Mathf.RoundToInt(position.width);
                for (int i = settings.Step; i < widthI; i += settings.Step)
                {
                    AddPoint(i);
                }
                AddPoint(widthI);
                Draw.Path(path);
                Draw.PopState();
            }

            var mouseRaw = Event.current.mousePosition;
            var mouseLocal = GlobalToLocal(mouseRaw);
            var snappedMouseLocal = new Vector2(Snap(mouseLocal.x), mouseLocal.y);
            float indicatorX = LocalToGlobal(snappedMouseLocal).x;
            if (position.Contains(mouseRaw))
            {
                /* Indicator line */
                {
                    Draw.PushState();
                    Draw.Fill = Color.clear;
                    Draw.Stroke = Color.white;
                    Draw.StrokeWeight = 1.5f;
                    Draw.Line(indicatorX, position.yMin, indicatorX, position.yMax);
                    Draw.PopState();
                }
                var x = snappedMouseLocal.x;
                var y = function(x);
                /* Indicator dot */
                {
                    Draw.PushState();
                    Draw.Fill = Color.white;
                    Draw.Stroke = Color.clear;
                    Draw.Circle(LocalToGlobal(new Vector2(x, y)), settings.LineThickness * 4);
                    Draw.PopState();
                }
                /* Values label */
                {
                    Draw.PushState();
                    Draw.Fill = settings.LabelBackgroundColor;
                    Draw.Stroke = Color.clear;
                    Draw.AntiAliasing = 0;
                    var xContent = new GUIContent(string.Format(settings.XFormat, x));
                    var yContent = new GUIContent(string.Format(settings.YFormat, y));
                    var xSize = Vector2Int.CeilToInt(settings.LabelStyle.CalcSize(xContent));
                    var ySize = Vector2Int.CeilToInt(settings.LabelStyle.CalcSize(yContent));
                    var boxWidth = Mathf.CeilToInt(Mathf.Max(xSize.x, ySize.x));
                    var boxHeight = Mathf.CeilToInt(xSize.y + ySize.y);
                    var boxPosition = new Rect(position.x, position.y, boxWidth, boxHeight);
                    Draw.Rect(boxPosition);
                    {
                        var xBoxPosition = boxPosition;
                        xBoxPosition.height = xSize.y;
                        GUI.Label(xBoxPosition, xContent, settings.LabelStyle);
                    }
                    {
                        var yBoxPosition = boxPosition;
                        yBoxPosition.y += xSize.y;
                        yBoxPosition.height = ySize.y;
                        GUI.Label(yBoxPosition, yContent, settings.LabelStyle);
                    }
                    Draw.PopState();
                }
            }
        }
    }
}
