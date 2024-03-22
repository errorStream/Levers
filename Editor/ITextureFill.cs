using UnityEngine;

namespace Levers
{
    /// <summary>
    /// Settings for how a texture should be used to fill a shape
    /// </summary>
    public interface ITextureFill
    {
        /// <summary>
        /// The texture to use
        /// </summary>
        Texture2D Texture { get; }
        /// <summary>
        /// The size of the texture
        /// </summary>
        Vector2 Size { get; }
        /// <summary>
        /// Where the center of the texture should be in space
        /// </summary>
        Vector2 Center { get; set; }
        /// <summary>
        /// The scale of the texture
        /// </summary>
        Vector2 Scale { get; set; }
        /// <summary>
        /// The rotation of the texture in radians
        /// </summary>
        float Rotation { get; set; }
    }
}
