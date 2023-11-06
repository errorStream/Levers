using System.Collections.Generic;
using UnityEngine;

namespace Levers
{
    public interface IGradient
    {
        Texture2D Texture { get; }
        Vector2 Size { get; }
        Vector2 Center { get; set; }
        Vector2 Scale { get; set; }
        float Rotation { get; set; }
    }
    public class LinearGradient : IGradient
    {
        public Texture2D Texture { get; }
        public Vector2 Size { get; }
        public Vector2 Center { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public LinearGradient(IReadOnlyList<ColorStop> stops, Vector2 size, int detail = 128)
        {
            Texture = GradientGenerator.GenerateLinearGradient(detail, stops);
            Size = size;
        }
        ~LinearGradient()
        {
            Object.DestroyImmediate(Texture);
        }
    }
    public class RadialGradient : IGradient
    {
        public Texture2D Texture { get; }
        public Vector2 Size { get; }
        public Vector2 Center { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public RadialGradient(IReadOnlyList<ColorStop> stops, Vector2 size, int detail = 128)
        {
            Texture = GradientGenerator.GenerateRadialGradient(detail, stops);
            Size = size;
        }
        ~RadialGradient()
        {
            Object.DestroyImmediate(Texture);
        }
    }
}
