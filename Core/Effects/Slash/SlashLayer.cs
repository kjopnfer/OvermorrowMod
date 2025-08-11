using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OvermorrowMod.Core.Effects.Slash
{
    /// <summary>
    /// Defines a single visual layer of a slash effect
    /// </summary>
    public struct SlashLayer
    {
        /// <summary>
        /// Texture to use for this layer
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// Color tint for this layer
        /// </summary>
        public Color Color;

        /// <summary>
        /// Width scale multiplier for this layer (1.0 = same as base width)
        /// </summary>
        public float WidthScale;

        /// <summary>
        /// Opacity of this layer (0.0 to 1.0)
        /// </summary>
        public float Opacity;

        /// <summary>
        /// Blend state for rendering this layer
        /// </summary>
        public BlendState BlendState;

        /// <summary>
        /// Texture flipping options
        /// </summary>
        public SpriteEffects SpriteEffects;

        /// <summary>
        /// Optional width curve function for variable width along the slash
        /// If null, uses constant width
        /// </summary>
        public Func<float, float> WidthCurve;

        /// <summary>
        /// Creates a basic slash layer
        /// </summary>
        public SlashLayer(Texture2D texture, Color color, float widthScale = 1f, float opacity = 1f)
        {
            Texture = texture;
            Color = color;
            WidthScale = widthScale;
            Opacity = opacity;
            BlendState = BlendState.AlphaBlend;
            SpriteEffects = SpriteEffects.None;
            WidthCurve = null;
        }

        public SlashLayer(Texture2D texture, Color color, float widthScale, float opacity, BlendState blendState, SpriteEffects spriteEffects = SpriteEffects.None, Func<float, float> widthCurve = null)
        {
            Texture = texture;
            Color = color;
            WidthScale = widthScale;
            Opacity = opacity;
            BlendState = blendState;
            SpriteEffects = spriteEffects;
            WidthCurve = widthCurve;
        }

        /// <summary>
        /// Gets the final color with opacity applied
        /// </summary>
        public Color FinalColor => Color * Opacity;

        /// <summary>
        /// Calculates the actual width for this layer given a base width
        /// </summary>
        public float GetWidth(float baseWidth, float t = 0f)
        {
            float scaledWidth = baseWidth * WidthScale;

            if (WidthCurve != null)
            {
                return WidthCurve(t);
            }

            return scaledWidth;
        }
    }
}