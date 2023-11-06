using System.Collections.Generic;
using UnityEngine;

namespace Levers
{
    /// <summary>
    /// A utility class for generating gradient textures.
    /// </summary>
    internal static partial class GradientGenerator
    {
        /// <summary>
        /// Generates a linear gradient texture with the specified width, height, and color stops.
        /// </summary>
        /// <param name="size">The width of the texture.</param>
        /// <param name="colorStops">A list of color stops defining the gradient.</param>
        /// <returns>A Texture2D object containing the linear gradient.</returns>
        internal static Texture2D GenerateLinearGradient(int size, IReadOnlyList<ColorStop> colorStops)
        {
            if (colorStops == null || colorStops.Count < 2)
            {
                Debug.LogWarning("Gradient must have at least two color stops.");
                return null;
            }

            var width = size;
            var height = 1;
            Texture2D gradientTexture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Color[] gradientColors = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                float t = (float)x / (width - 1);
                Color gradientColor = CalculateGradientColor(t, colorStops);
                gradientColors[x] = gradientColor;
            }

            gradientTexture.SetPixels(gradientColors);
            gradientTexture.Apply();

            return gradientTexture;
        }

        /// <summary>
        /// Generates a radial gradient texture with the specified width, height, and color stops.
        /// </summary>
        /// <param name="size">The size of each axis the texture.</param>
        /// <param name="colorStops">A list of color stops defining the gradient.</param>
        /// <returns>A Texture2D object containing the radial gradient.</returns>
        internal static Texture2D GenerateRadialGradient(int size, IReadOnlyList<ColorStop> colorStops)
        {
            if (colorStops == null || colorStops.Count < 2)
            {
                Debug.LogWarning("Gradient must have at least two color stops.");
                return null;
            }

            Texture2D gradientTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Color[] gradientColors = new Color[size * size];

            float center = (size - 1) * 0.5f;
            float maxRadius = center;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float radius = Mathf.Sqrt((dx * dx) + (dy * dy));
                    float t = Mathf.Clamp01(radius / maxRadius);

                    Color gradientColor = CalculateGradientColor(t, colorStops);
                    gradientColors[(y * size) + x] = gradientColor;
                }
            }

            gradientTexture.SetPixels(gradientColors);
            gradientTexture.Apply();

            return gradientTexture;
        }

        private static Color CalculateGradientColor(float t, IReadOnlyList<ColorStop> colorStops)
        {
            for (int i = 0; i < colorStops.Count - 1; i++)
            {
                if (t >= colorStops[i].Position && t <= colorStops[i + 1].Position)
                {
                    float localT = Mathf.InverseLerp(colorStops[i].Position, colorStops[i + 1].Position, t);
                    return Color.Lerp(colorStops[i].Color, colorStops[i + 1].Color, localT);
                }
            }

            return Color.black;
        }
    }
}
