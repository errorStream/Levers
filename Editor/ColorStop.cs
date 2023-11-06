using UnityEngine;

namespace Levers
{
    /// <summary>
    /// Represents a color stop in a gradient.
    /// </summary>
    public struct ColorStop
    {
        private float _position;
        /// <summary>
        /// The position of the color stop in the gradient, ranging from 0 to 1.
        /// </summary>
        public float Position
        {
            get { return Mathf.Clamp01(_position); }
            set { _position = Mathf.Clamp01(value); }
        }
        /// <summary>
        /// The color of the color stop.
        /// </summary>
        public Color Color;

        /// <summary>
        /// Creates a new ColorStop instance with the specified position and color.
        /// </summary>
        /// <param name="position">The position of the color stop, ranging from 0 to 1.</param>
        /// <param name="color">The color of the color stop.</param>
        public ColorStop(float position, Color color)
        {
            _position = Mathf.Clamp01(position);
            Color = color;
        }
    }
}
