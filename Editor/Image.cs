using UnityEngine;

namespace Levers
{
    /// <summary>
    /// An image based texture fill
    /// </summary>
    public class Image : ITextureFill
    {
        /// <inheritdoc />
        public Texture2D Texture { get; }
        /// <inheritdoc />
        public Vector2 Size => new Vector2(Texture.width, Texture.height);
        /// <inheritdoc />
        public Vector2 Center { get; set; }
        /// <inheritdoc />
        public Vector2 Scale { get; set; } = Vector2.one;
        /// <inheritdoc />
        public float Rotation { get; set; }

        /// <summary>
        /// Create a new image texture fill
        /// </summary>
        /// <param name="texture">The texture to use</param>
        public Image(Texture2D texture)
        {
            Texture = texture;
        }
    }
}
