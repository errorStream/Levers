using System.Collections.Generic;
using UnityEngine;

namespace Levers
{
    /// <summary>
    /// A linear gradient texture fill
    /// </summary>
    public class LinearGradient : ITextureFill
    {
        /// <inheritdoc />
        public Texture2D Texture { get; }
        /// <inheritdoc />
        public Vector2 Size { get; }
        /// <inheritdoc />
        public Vector2 Center { get; set; }
        /// <inheritdoc />
        public Vector2 Scale { get; set; } = Vector2.one;
        /// <inheritdoc />
        public float Rotation { get; set; }
        /// <summary>
        /// Create a new linear gradient texture fill
        /// </summary>
        /// <param name="stops">The color stops</param>
        /// <param name="size">The size of the gradient</param>
        /// <param name="detail">The resolution of the gradient</param>
        public LinearGradient(IReadOnlyList<ColorStop> stops, Vector2 size, int detail = 128)
        {
            Texture = GradientGenerator.GenerateLinearGradient(detail, stops);
            Size = size;
        }
        /// <summary>
        /// Destructor for the linear gradient
        /// </summary>
        ~LinearGradient()
        {
            Object.DestroyImmediate(Texture);
        }
    }
    /// <summary>
    /// A radial gradient texture fill
    /// </summary>
    public class RadialGradient : ITextureFill
    {
        /// <inheritdoc />
        public Texture2D Texture { get; }
        /// <inheritdoc />
        public Vector2 Size { get; }
        /// <inheritdoc />
        public Vector2 Center { get; set; }
        /// <inheritdoc />
        public Vector2 Scale { get; set; } = Vector2.one;
        /// <inheritdoc />
        public float Rotation { get; set; }
        /// <summary>
        /// Create a new radial gradient texture fill
        /// </summary>
        /// <param name="stops">The color stops</param>
        /// <param name="size">The size of the gradient</param>
        /// <param name="detail">The resolution of the gradient</param>
        public RadialGradient(IReadOnlyList<ColorStop> stops, Vector2 size, int detail = 128)
        {
            Texture = GradientGenerator.GenerateRadialGradient(detail, stops);
            Size = size;
        }
        /// <summary>
        /// Destructor for the radial gradient
        /// </summary>
        ~RadialGradient()
        {
            Object.DestroyImmediate(Texture);
        }
    }
}
