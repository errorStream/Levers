using UnityEngine;

namespace Levers
{
    public static partial class Components
    {
        private static Color _colorScheme1 = new Color32(251, 187, 173, 255);
        private static Color _colorScheme2 = new Color32(238, 134, 149, 255);
        private static Color _colorScheme3 = new Color32(74, 122, 150, 255);
        private static Color _colorScheme4 = new Color32(51, 63, 88, 255);
        private static Color _colorScheme5 = new Color32(41, 40, 49, 255);

        private static GUIStyle _textStyle = null;

        public static Vector2 PointInUnitCircle(Rect position, Vector2 point)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = Event.current.GetTypeForControl(controlID);
            Vector2 flippedPoint = new Vector2(point.x, -point.y);
            Vector2 res = point;
            const int padding = 5;
            var circleSize = Mathf.Min(position.width - padding, position.height - padding);
            var center = new Vector2(position.x + (position.width / 2),
                                     position.y + (position.height / 2));
            var handleSize = circleSize / 12f;
            var maxDist = (circleSize / 2) - (handleSize / 2);
            bool MouseOverControl(Vector2 mousePosition)
            {
                var mouseFromCenter = mousePosition - center;
                return mouseFromCenter.magnitude <= circleSize / 2;
            }
            if (_textStyle is null)
            {
                _textStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    fontSize = 10
                };
                _textStyle.normal.textColor = _colorScheme3;
            }
            switch (eventType)
            {
                case EventType.Repaint:
                    {
                        Draw.PushState();
                        {
                            /* Background */
                            if (point.magnitude >= (2047f / 2048f))
                            {
                                Draw.Fill = _colorScheme3;
                            }
                            else
                            {
                                Draw.Fill = _colorScheme4;
                            }
                            Draw.Stroke = Color.clear;
                            Draw.Ellipse(center, circleSize * Vector2.one);
                            Draw.Fill = _colorScheme5;
                            Draw.Ellipse(center, (circleSize - (handleSize * 2) + 1) * Vector2.one);
                        }
                        {
                            /* Handle */
                            if (GUIUtility.hotControl == controlID)
                            {
                                Draw.Fill = _colorScheme1;
                            }
                            else
                            {
                                Draw.Fill = _colorScheme2;
                            }
                            flippedPoint = Vector2.ClampMagnitude(flippedPoint, 1);
                            var handleCenter = center + (flippedPoint * maxDist);
                            Draw.Ellipse(handleCenter, handleSize * Vector2.one);
                        }
                        {
                            /* Text */
                            var msg = $"{point.x:F2}, {point.y:F2}";
                            GUI.Label(position, msg, _textStyle);
                        }
                        Draw.PopState();
                        break;
                    }
                case EventType.MouseDown:
                    {
                        if (MouseOverControl(Event.current.mousePosition) && Event.current.button == 0)
                        {
                            GUIUtility.hotControl = controlID;
                        }

                        break;
                    }

                case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                        }

                        break;
                    }
                default:
                    break;
            }

            if (Event.current.isMouse && GUIUtility.hotControl == controlID)
            {
                var mouseFromCenter = Event.current.mousePosition - center;
                res = Vector2.ClampMagnitude(mouseFromCenter / maxDist, 1);
                res.y = -res.y;
                GUI.changed = true;
                Event.current.Use();
            }

            return res;
        }
    }
}
