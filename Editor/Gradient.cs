using System.Collections.Generic;
using UnityEngine;

namespace Levers
{
    public class LinearGradient : ITextureFill
    {
        public Texture2D Texture { get; }
        public Vector2 Size { get; }
        public Vector2 Center { get; set; }
        public Vector2 Scale { get; set; } = Vector2.one;
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
    public class RadialGradient : ITextureFill
    {
        public Texture2D Texture { get; }
        public Vector2 Size { get; }
        public Vector2 Center { get; set; }
        public Vector2 Scale { get; set; } = Vector2.one;
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
