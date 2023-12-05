using UnityEngine;

namespace Levers
{
    public class Image : ITextureFill
    {
        public Texture2D Texture { get; }

        public Vector2 Size => new Vector2(Texture.width, Texture.height);

        public Vector2 Center { get; set; }
        public Vector2 Scale { get; set; } = Vector2.one;
        public float Rotation { get; set; }

        public Image(Texture2D texture)
        {
            Texture = texture;
        }
    }
}
