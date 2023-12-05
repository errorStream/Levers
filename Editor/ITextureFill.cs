using UnityEngine;

namespace Levers
{
    public interface ITextureFill
    {
        Texture2D Texture { get; }
        Vector2 Size { get; }
        Vector2 Center { get; set; }
        Vector2 Scale { get; set; }
        float Rotation { get; set; }
    }
}
