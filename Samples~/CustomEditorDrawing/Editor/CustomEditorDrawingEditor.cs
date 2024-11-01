using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Levers;

[CustomEditor(typeof(CustomEditorDrawing))]
public class CustomEditorDrawingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("With clipping");
        {
            var position = GUILayoutUtility.GetRect(500, 40);
            GUI.BeginGroup(position);
            {
                GUI.Box(new Rect(0, 0, 800, 600), "");
                Draw.PushState();
                {
                    var path = new Path2D(new Vector2(7, 37));
                    path.LineTo(new Vector2(47, 2));
                    path.LineTo(new Vector2(112, 57));
                    path.LineTo(new Vector2(22, 57));
                    path.LineTo(new Vector2(87, 2));
                    path.LineTo(new Vector2(127, 37));
                    Draw.Fill = Color.green;
                    Draw.Stroke = Color.clear;
                    Draw.Path(path);
                }
                Draw.PopState();
            }
            GUI.EndGroup();
        }
        GUILayout.Label("Without clipping");
        {
            var position = GUILayoutUtility.GetRect(500, 40);
            var originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Translate(position.min) * originalMatrix;
            {
                GUI.Box(new Rect(0, 0, 800, 600), "");
                Draw.PushState();
                {
                    var path = new Path2D(new Vector2(7, 37));
                    path.LineTo(new Vector2(47, 2));
                    path.LineTo(new Vector2(112, 57));
                    path.LineTo(new Vector2(22, 57));
                    path.LineTo(new Vector2(87, 2));
                    path.LineTo(new Vector2(127, 37));
                    Draw.Fill = Color.green;
                    Draw.Stroke = Color.clear;
                    Draw.Path(path);
                }
                Draw.PopState();
            }
            GUI.matrix = originalMatrix;
        }
    }
}
